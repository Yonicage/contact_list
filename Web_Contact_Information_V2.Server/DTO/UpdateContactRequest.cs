/*
 * File: UpdateContactRequest.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class represents a given request from a user 
 * that updates a new contact.
 */
using System.ComponentModel.DataAnnotations;

namespace Web_Contact_Information_V2.Server.DTO
{
    /// <summary>
    /// The contact request class representing the 
    /// values needed to update a new contact.
    /// </summary>
    public class UpdateContactRequest
    {
        /// <summary>
        /// The ID of the contact to be 
        /// updated.
        /// It is an obligatory field.
        /// </summary>
        public int ContactID { get; set; }

        /// <summary>
        /// The name of the updated contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The phone of the updated contact.
        /// It is an obligatory field.
        /// </summary>
        public required string Phone { get; set; }

        /// <summary>
        /// The fax of the updated contact.
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// The email of the updated contact.
        /// </summary>
        public string? eMail { get; set; }

        /// <summary>
        /// The notes of the updated contact.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The username of the user that made the 
        /// update contact request.
        /// It is an obligatory field.
        /// </summary>
        public required string User { get; set; }
    }
}
