/*
* File: App.jsx
* Author: Yoniel Ruiz Alfaro
* Date: 09/02/2026
* Purpose: This react jsx file represents the main screen for the application.
* It shows the login and main contacts screen.
*
* WindowComponent author: Nathraktim
*
*/

import WindowComponent from 'react-flexi-window';
import { useState } from "react";


//This function creates an instance of the contacts screen
//It takes parameters from the main app screen
function ContactScreen({ contact, username, onUpdated, onCancel }) {

    const [name, setName] = useState(contact?.name || "");          //Updated/new name
    const [phone, setPhone] = useState(contact?.phone || "");       //Updated/new phone
    const [fax, setFax] = useState(contact?.fax || "");             //Updated/new fax
    const [email, setEmail] = useState(contact?.eMail || "");       //Updated/new email
    const [notes, setNotes] = useState(contact?.notes || "");       //Updated/new notes
    const isEditing = contact.contactID !== undefined;              //Used to check if updating or creating a contact
    const [fieldErrors, setFieldErrors] = useState({});             //An array representing each error caught during validation

    //This function handles the contact
    //It validates each contact field provided by the user
    //It decides if the user is editing or creating a new contact
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
                onCancel()
            } else {
                const error = await response.text();
                alert("Could not edit contact: ", {error})
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
                onCancel()
            } else {
                const error = await response.text();
                alert("Could not edit contact: ", {error})
            }
        }
       

        
    }


    //Validates each field according to the contact requirements
    //returns an object of all the errors present.
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

        setFieldErrors(errors); //set the field error object to track the present errors

        return Object.keys(errors).length === 0;

    }
        

    //Main content of the contact screen
    //Uses windowcomponent for a draggable and resizable screen.
    return (
        <div style={{ position: "fixed", top: "25%", left: "25%"}}>
            <WindowComponent
                w={800}
                h={500}
                windowColor="gray-50"
                windowBorder={2}
                windowBorderColor="red"
                windowBorderRadius="10g"
            >
                <div className="contact-header">
                    <h2>CONTACT</h2>
                    <a href="#" onClick={handleContact}>{isEditing ? "Update" : "Add"}</a>
                    <p>|</p>
                    <a href="#" onClick={onCancel}>Cancel</a>
                </div>
                

                <div className="input-section">
                    <label>Name*: </label>
                    <input
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        className={fieldErrors.name ? "input-error" : ""}
                    />
                </div>
                <div className="input-section">
                    <label>Phone*: </label>
                    <input
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        className={fieldErrors.phone ? "input-error" : ""}
                    />

                </div>

                <div className="input-section">
                <label>Fax: </label>
                <input
                    value={fax}
                    onChange={(e) => setFax(e.target.value)}
                    className={fieldErrors.fax ? "input-error" : ""}
                        />
                
                    </div>

                <div className="input-section">
                <label>Email: </label>
                <input
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                        className={fieldErrors.email ? "input-error" : ""}
                    />
                </div>
                <div className="input-section">
                    <label>Notes: </label>
                    <textarea
                        value={notes}
                        onChange={(e) => setNotes(e.target.value)}
                    />
                </div>
              
            </WindowComponent>

        </div>
    );

}

export default ContactScreen;