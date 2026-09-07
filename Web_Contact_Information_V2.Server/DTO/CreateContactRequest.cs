/*
 * File: CreateContactRequest.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class represents a given request from a user 
 * that creates a new contact.
 */


namespace Web_Contact_Information_V2.Server.DTO
{
    /// <summary>
    /// The contact request class representing the 
    /// values needed to create a new contact.
    /// </summary>
    public class CreateContactRequest
    {
        /// <summary>
        /// The name of the new contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The phone of the new contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Phone { get; set; }

        /// <summary>
        /// The fax of the new contact.
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// The email of the new contact.
        /// </summary>
        public string? eMail { get; set; }

        /// <summary>
        /// The notes of the new contact.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The username of the user that made the 
        /// new contact request.
        /// It is an obligatory field.
        /// </summary>
        public required string User { get; set; }
    }
}
