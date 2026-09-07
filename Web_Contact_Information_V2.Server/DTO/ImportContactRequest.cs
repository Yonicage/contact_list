/*
 * File: ImportContactRequest.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class represents a single contact in the 
 * lists of contacts from import contacts request.
 */


namespace Web_Contact_Information_V2.Server.DTO
{
    
    /// <summary>
    /// The contact request class representing the 
    /// values needed to import a contact.
    /// </summary>
    public class ImportContactRequest
    {
        /// <summary>
        /// The name of the imported contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Name { get; set; }

        // <summary>
        /// The phone of the imported contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Phone { get; set; }

        /// <summary>
        /// The fax of the imported contact.
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// The email of the imported contact.
        /// </summary>
        public string? eMail { get; set; }

        /// <summary>
        /// The notes of the imported contact.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The last update date of the imported contact.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        


    }
}
