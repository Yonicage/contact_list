/*
 * File: ImportContactsRequest.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class represents a given request from a user 
 * that imports multiple contacts. 
 */

namespace Web_Contact_Information_V2.Server.DTO
{
    /// <summary>
    /// The contact request class representing how 
    /// the request is structured when given from 
    /// the client.
    /// </summary>
    public class ImportContactsRequest
    {
        /// <summary>
        /// A list of contacts to import
        /// </summary>
        public required List<ImportContactRequest> Contacts { get; set; }

        /// <summary>
        /// The username of the user that made the 
        /// import contact request.
        /// It is an obligatory field.
        /// </summary>
        public required string User { get; set; }

    }
}
