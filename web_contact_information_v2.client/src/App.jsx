import { useEffect, useState } from 'react';
import './App.css';
import ContactScreen from "./ContactScreen";

function App() {
    const [username, setUsername] = useState("");
    const [error, setError] = useState();
    const [loggedIn, setLoggedIn] = useState(false);
    const [contacts, setContacts] = useState([]);   
    const [filter, setFilter] = useState("");
    const [selectedContact, setSelectedContact] = useState(null);
    const [message, setMessage] = useState("");


    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/api/Contact`)
            .then(response => response.json())
            .then(data => {
                setContacts(data);
            });
    },[]);

    function handleLogin() {
        const trimmedUsername = username.trim();

        if (trimmedUsername === "") {
            setError("Username is Required!");
            return;
        }
        setUsername(trimmedUsername);
        setLoggedIn(true);
        setError("");
    }



    function formatNumber(number) {
        if (!number) return "";
        
        return `(${number.substring(0,3)}) ${number.substring(3, 6)}-${number.substring(6)}`;
    }

    function formatDate(date) {
        const d = new Date(date)

        const day = String(d.getDay()).padStart(2, "0");
        const month = String(d.getMonth() + 1).padStart(2, "0")
        const year = d.getFullYear();

        let hours = d.getHours();
        const minutes = String(d.getMinutes()).padStart(2, "0");

        const period = hours >= 12 ? "PM" : "AM";

        hours = hours % 12; //handling military hour
        if (hours === 0) hours == 12;

        hours = String(hours).padStart(2, "0");

        return `${day}/${month}/${year} ${hours}:${minutes} ${period}`

    }

    function getContacts() {
        fetch(`${import.meta.env.VITE_API_URL}/api/Contact?filter=${encodeURIComponent(filter)}`)
            .then(response => response.json())
            .then(data => {
                setContacts(data);
            });
    }

    function handleView(contact) {
        setSelectedContact(contact);

    }
    function handleExit() {
        setLoggedIn(false);
        setUsername("");
    }

    function exportXML() {
        const xml = document.implementation.createDocument("", "Contacts");

        contacts.forEach(contact => {
            const contactElement = xml.createElement("Contact");

            const contactID = xml.createElement("ContactID");
            contactID.textContent = contact.contactID;
            contactElement.appendChild(contactID);

            const name = xml.createElement("Name");
            name.textContent = contact.name;
            contactElement.appendChild(name);

            const phone = xml.createElement("Phone");
            phone.textContent = contact.phone;
            contactElement.appendChild(phone);

            const fax = xml.createElement("Fax");
            fax.textContent = contact.fax;
            contactElement.appendChild(fax);

            const eMail = xml.createElement("eMail");
            eMail.textContent = contact.eMail;
            contactElement.appendChild(eMail);

            const notes = xml.createElement("Notes");
            notes.textContent = contact.notes;
            contactElement.appendChild(notes);

            const lastUpdateDate = xml.createElement("LastUpdateDate");
            lastUpdateDate.textContent = contact.lastUpdateDate;
            contactElement.appendChild(lastUpdateDate);

            const lastUpdateUserName = xml.createElement("LastUpdateUserName");
            lastUpdateUserName.textContent = contact.lastUpdateUserName;
            contactElement.appendChild(lastUpdateUserName);

            xml.documentElement.appendChild(contactElement);


        });
        const serializer = new XMLSerializer();
        const xmlString = serializer.serializeToString(xml);

        const blob = new Blob([xmlString], { type: "application/xml" });
        const url = URL.createObjectURL(blob);

        const link = document.createElement("a");
        link.href = url;
        link.download = "Contacts.xml";
        link.click();
        URL.revokeObjectURL(url);
    }

    function handleCSVImport(event) {
        const file = event.target.files[0];
        const importedContacts = [];
        if (!file) {
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const csvText = e.target.result;
            const rows = csvText.split(/\r?\n/);
            for (let i = 1; i<rows.length; i++) {
                if (rows[i].trim() === "") {
                    continue;//empty fields
                }

                const fields = parseCSVRow(rows[i]);
                const contact = createContact(fields);

                importedContacts.push(contact);

            }

            const importRequest = {
                contacts: importedContacts,
                user: username
            };
            fetch(`${import.meta.env.VITE_API_URL}/api/Contact/import`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(importRequest)
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Import failed");
                    }

                    return response.json();
                })
                .then(data => {
                    alert("Contacts imported successfully: ",data);
                    getContacts();
                })
                .catch(error => {
                    console.error("Import error: ", error);
                    alert("Failed to import contacts.");
                })
            
        };
        reader.readAsText(file);
    }

    function createContact(fields) {
        const dateParts = fields[5].split(" ");
        const timeParts = dateParts[1].split(":");

        const formattedDate =
            `${dateParts[0]}T${timeParts[0].padStart(2, "0")}:${timeParts[1]}:${timeParts[2]}`;
        return {
            name: fields[0],
            phone: fields[1],
            fax: fields[2],
            eMail: fields[3],
            notes: fields[4],
            lastUpdateDate: formattedDate
        };
    }

    function parseCSVRow(row) {
        const fields = [];
        let currentField = "";
        let insideQuotes = false;

        for (let i = 0; i < row.length; i++) {
            const character = row[i];

            if (character === '"') {
                insideQuotes = !insideQuotes;
            }
            else if (character === "," && !insideQuotes) {
                fields.push(currentField);
                currentField = "";
            }
            else {
                currentField += character;
            }
        }

        fields.push(currentField);

        return fields;
    }

    if (loggedIn) {


        return (
            
            
            <div className="main-screen">
                <div className="top-section">
                    <div>
                    <h3>myContacts | Lookup</h3>
                    </div>
                    <div className="user-section">
                        <p>Welcome {username}</p>
                        <a href="#" onClick={handleExit}>EXIT</a>
 
                    </div>

                </div>
               
                <div className="toolbar">
                    <div className="filter-section">
                        <p>Filter: </p>
                        <input
                            type="text"
                            value={filter}
                            onChange={(e) => setFilter(e.target.value)}
                        />
                        <button onClick={getContacts}>Search</button>
                    </div>
                    <div className="action-section">
                        <a href="#" onClick={exportXML}>Export</a>
                        <a href="#" onClick={() => document.getElementById("csvFileInput").click()}>Import</a>
                        <a href="#" onClick={() => setSelectedContact({})}>+ Add New</a>

                    </div>


                </div>   
             
                <input
                    type="file"
                    accept=".csv"
                    id="csvFileInput"
                    style={{ display: "none" }}
                    onChange={handleCSVImport}
                />
                <div className="results-header">
                    <h5>Search Results</h5>
                    <hr />
                </div>
               
                {message && (
                    <div className="action-message">
                        {message}
                    </div>
                )}
               
                <table className="contact-table">
                    <thead>
                        <tr>
                            <th></th>
                            <th>Name</th>
                            <th>Phone</th>
                            <th>Fax</th>
                            <th>eMail</th>
                            <th>Last Update</th>
                        </tr>
                    </thead>
                    <tbody>
                        {contacts.map(contact => (
                            <tr key={contact.contactID}>
                                <td>
                                    <a href="#" onClick={() => handleView(contact)}>View</a>
                                </td>
                                <td>{contact.name}</td>
                                <td>{formatNumber(contact.phone)}</td>
                                <td>{formatNumber(contact.fax)}</td>
                                <td>{contact.eMail}</td>
                                <td>{formatDate(contact.lastUpdateDate)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {selectedContact && (
                    <ContactScreen
                        contact={selectedContact}
                        username={username}
                        onUpdated={(action) => {
                            getContacts();
                            setMessage(
                                action === "add"
                                    ? "Contact added successfully"
                                    : "Contact updated successfully"
                            );
                            setTimeout(() => {
                                setMessage("");
                            }, 5000);
                        }}
                        onCancel={() => setSelectedContact(null)}
                      
                    />
                )}
            </div>

        )
    }
    //Entry Point of our Front End


    return (
        <div className="login-screen">
            <div className="login-box">

                <title>Login</title>
                <p>myContacts v.1.1</p>
                <label>Username</label>
                <input
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />
                <button onClick={handleLogin}>ENTER</button>
                <p>{error}</p>

            </div>
          
            
        </div>
    );
    
}

export default App;