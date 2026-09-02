import WindowComponent from 'react-flexi-window';
import { useState } from "react";
function ContactScreen({ contact, username, onUpdated, onCancel }) {

    const [name, setName] = useState(contact?.name || "");
    const [phone, setPhone] = useState(contact?.phone || "");
    const [fax, setFax] = useState(contact?.fax || "");
    const [email, setEmail] = useState(contact?.eMail || "");
    const [notes, setNotes] = useState(contact?.notes || "");
    const isEditing = contact.contactID !== undefined;
    const [fieldErrors, setFieldErrors] = useState({});

    async function handleContact() {


        if (!validateContact()) {
            return;
        }

        if (isEditing) {
            const updatedContact = {
                contactID: contact.contactID,
                name: name,
                phone: phone,
                fax: fax,
                eMail: email,
                notes: notes,
                user: username
            };
            const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Contact`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(updatedContact)
            });

            if (response.ok) {
                onUpdated("update");
            } else {
                const error = await response.text();
                console.log("PUT error: ", error)
            }
        } else {
            const newContact = {
                name: name,
                phone: phone,
                fax: fax,
                eMail: email,
                notes: notes,
                user: username
            };
            const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Contact`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(newContact)

            });
          
            if (response.ok) {
                onUpdated("add");
            } else {
                const error = await response.text();
                console.log("POST error: ",error)
            }
        }
       

        
    }

    function validateContact() {
        const errors = {};
        if (name.trim() === "") {
            alert("Name is required.");
            errors.name = true;
            
        }

        if (name.length > 50) {
            alert("Name cannot exceed 50 characters.");
            errors.name = true;
           
        }

        if (phone.trim() === "") {
            alert("Phone is required.");
            errors.phone = true;
            
        }

        if (!/^\d{10}$/.test(phone)) {
            alert("Phone must contain exactly 10 digits.");
            errors.phone = true;

           
        }

        if (fax !== "" && !/^\d{10}$/.test(fax)) {
            alert("Fax must contain exactly 10 digits.");
            errors.fax = true;
            
        }

        if (email !== "" && email.length > 50) {
            alert("Email cannot exceed 50 characters.");
            errors.email = true;
            
        }

        if (email !== "" && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) {
            alert("Invalid email format.");
            errors.email = true;
            
        }

        setFieldErrors(errors);

        return Object.keys(errors).length === 0;

    }
        


    return (
        <div style={{ position: "fixed", top: "25%", left: "25%"}}>
            <WindowComponent
            w={800}
            h={500}
            windowColor="-500"
            windowBorderRadius="10g"
            >
                <h2>Contact Information</h2>

                <label>Name: </label>
                <input
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    className={fieldErrors.name ? "input-error" : ""}
                />
                <label>Phone: </label>
                <input
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    className={fieldErrors.phone ? "input-error" : ""}
                />
                <label>Fax: </label>
                <input
                    value={fax}
                    onChange={(e) => setFax(e.target.value)}
                    className={fieldErrors.fax ? "input-error" : ""}
                />
                <label>Email: </label>
                <input
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className={fieldErrors.email ? "input-error" : ""}
                />
                <label>Notes: </label>
                <input
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                />
              

                <button onClick={handleContact}>{isEditing ? "Update" : "Add"}</button>
                <button onClick={onCancel}>Cancel</button>

            </WindowComponent>

        </div>
    );

}


export default ContactScreen;