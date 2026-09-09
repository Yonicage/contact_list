/*
* File: App.jsx
* Author: Yoniel Ruiz Alfaro
* Date: 09/02/2026
* Purpose: This react jsx file represents the main screen for the application.
* It shows the login and main contacts screen.
*/

import { useEffect, useState } from 'react';
import './App.css';
import ContactScreen from "./ContactScreen";

function App() {
    const [username, setUsername] = useState("");                  //Username of the user
    const [error, setError] = useState();                          //Handles error display
    const [loggedIn, setLoggedIn] = useState(false);               //Verifies if the user is logged in
    const [contacts, setContacts] = useState([]);                  //List of contacts, stores all contacts
    const [filter, setFilter] = useState("");                      //The filter used to search 
    const [selectedContact, setSelectedContact] = useState(null);  //The current selected contact to edit/update
    const [message, setMessage] = useState("");                    //Message that confirms if the contact has been edited/updated

    //Loads all contacts once when the page initially loads.
    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/api/Contact`)
            .then(response => response.json())
            .then(data => {
                setContacts(data);
            });
    },[]);


    //Verifies that the user has logged in. If it is true 
    //loads the main screen, else it shows the login screen.
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


    //Formats the number with a mask 777777777 ->(777) 777-7777
    function formatNumber(number) {
        if (!number) return "";
        
        return `(${number.substring(0,3)}) ${number.substring(3, 6)}-${number.substring(6)}`;
    }

    //Formats the date 2026-07-17T09:15:23 -> mm/dd/yyyy hh:mm pp
    function formatDate(date) {
        const year = date.substring(0, 4);
        const month = date.substring(5, 7);
        const day = date.substring(8, 10);
        const minutes = date.substring(14, 16);
        let hourInt = parseInt(date.substring(11, 13) - 4);  // subtract 4 to convert UTC to Puerto Rico time.

        if (hourInt < 0) hourInt += 24; //This approach may make it negative so we add 24 to compensate.

        const period = hourInt >= 12 ? "pm" : "am";

        hourInt = hourInt % 12; //Handle military time
        
        if (hourInt === 0) hourInt = 12;

        const hours = String(hourInt).padStart(2, "0");

        return `${day}/${month}/${year} ${hours}:${minutes} ${period}`;

    }

    //Gets the contacts when pressing search button according to the present filter, if filter
    //is empty it gets all of the contacts.
    function getContacts() {
        fetch(`${import.meta.env.VITE_API_URL}/api/Contact?filter=${encodeURIComponent(filter)}`)
            .then(response => response.json())
            .then(data => {
                setContacts(data);
            });
    }

    //When view link is pressed It sets the current selected contact
    //from the table.
    function handleView(contact) {
        setSelectedContact(contact);

    }

    //When exit link is pressed it sets login as "False" and 
    //sets the username to empty.
    function handleExit() {
        setLoggedIn(false);
        setUsername("");
    }

    //exports the loaded contact table to a xml file
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


    //Handles the import of a CSV file
    //Refresh contact table when imported
    function handleCSVImport(event) {
        const file = event.target.files[0];
        const importedContacts = [];
        if (!file) {
            alert("Unable to process file: File is not present");
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const csvText = e.target.result;
            const rows = csvText.split(/\r?\n/);
            for (let i = 1; i < rows.length; i++) { // Start at index 1 to skip the CSV header row
               
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
                    alert("Contacts imported successfully: ", data);
                    getContacts();
                })
                .catch(error => {
                    console.error("Import error: ", error);
                    alert("Failed to import contacts.");
                })
            
        };
        reader.readAsText(file);
    }

    //Parses one row from the supplied CSV file
    function parseCSVRow(row) {
        const fields = [];
        const allFields = row.split(",")
        let notesField = "";
        const lastField = allFields[allFields.length - 1]; 

        for (let i = 0; i < 4; i++) { //parse first 4 values (Name, Phone, Fax, email)
            const currentField = allFields[i];
            fields.push(currentField)
        }

        let commentBeginning = true;

        for (let i = 4; i < (allFields.length - 1); i++) { //parse the rest minus the last value (Notes)
            if (commentBeginning === true) {
                notesField += allFields[i];
                commentBeginning = false;
            } else {
                notesField += ","+ allFields[i];
            }
           
        }
        notesField = notesField.replace(/"/g, ""); //Remove all quotes from the notes
        fields.push(notesField);
        fields.push(lastField); //parse last value (Last Update Date)
        return fields;

    }


    //Creates the contact with the parsed fields
    //Maps each field to the contact property
    //Formats the date: 2026-07-18 8:30:00 -> 2026-07-17T08:30:00
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

    

    //Handles the Main content screen. Only shows if user is logged in.
    //When a user is selected via the view link, the contact screen pops up.
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

                {/* If there is a message to display show it, otherwise don't */}
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

                {/* If a contact is selected, show the contact screen. */}
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

    
    //Entry point: Login Screen
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