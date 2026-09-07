namespace Web_Contact_Information_V2.Server.Model
/*
* File: Contact.cs
* Author: Yoniel Ruiz Alfaro
* Date: 09/02/2026
* Purpose: This class represents a contact.
*/
{

    public class Contact
    {
        /// <summary>
        /// Creates a contact with the given Id, name, phone, fax, email, notes, last updated date, and last updated username.
        /// </summary>
        /// <param name="contactId"> The Id of the contact.</param>
        /// <param name="name">Name of the contact.</param>
        /// <param name="phone">Phone of the contact.</param>
        /// <param name="fax">Fax of the contact.</param>
        /// <param name="eMail">email of the contact.</param>
        /// <param name="notes">Notes of the contact.</param>
        /// <param name="lastUpdateDate">The last date when this contact was updated/created.</param>
        /// <param name="lastUpdateUserName">The username of the user that updated/created the contact.</param>
        public Contact(int contactId, string name, string phone, string fax, string eMail, string notes, DateTime lastUpdateDate, string lastUpdateUserName)
        {
            ContactID = contactId;
            Name = name;
            Phone = phone;
            Fax = fax;
            this.eMail = eMail;
            Notes = notes;
            LastUpdateDate = lastUpdateDate;
            LastUpdateUserName = lastUpdateUserName;
        }
        /// <summary>
        /// The id of the contact.
        /// </summary>
        public int ContactID { get; set; }

        /// <summary>
        /// The name of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The phone of the contact.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The fax of the contact.
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// The email of the contact.
        /// </summary>
        public string? eMail { get; set; }

        /// <summary>
        /// The notes of the contact.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The last updated date of the contact.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// The username that last updated/created the contact.
        /// </summary>
        public string LastUpdateUserName { get; set; }

    }
}
