/*
 * File: ContactValidator.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This utility class contains contact validation methods.
 */

using System.Text.RegularExpressions;
namespace Web_Contact_Information_V2.Server.DTO
{
    /// <summary>
    /// A validation utility class for contact objects.
    /// </summary>
    public static class ContactValidator
    {
        /// <summary>
        /// Validates that the name is not empty and does not exceed 50 characters.
        /// </summary>
        /// <param name="name">The name to be validated.</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Name is required.";
            }

            if (name.Length > 50)
            {
                return "Name cannot exceed 50 characters.";
            }

            return null;
        }

        /// <summary>
        /// Validates that the phone is not empty, does not contain less or more than 10 digits, and that the format is correct.
        /// </summary>
        /// <param name="phone">The phone number to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return "Phone is required.";
            }

            if (!Regex.IsMatch(phone, @"^\d{10}$"))
            {
                return "Phone must contain exactly 10 digits.";
            }

            return null;
        }


        /// <summary>
        /// Validates that the fax is the correct format (exactly ten digits) if it is not empty.
        /// </summary>
        /// <param name="fax">The fax to be validated.</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateFax(string? fax)
        {
            if (string.IsNullOrWhiteSpace(fax))
            {
                return null;
            }

            if (!Regex.IsMatch(fax, @"^\d{10}$"))
            {
                return "Fax must contain exactly 10 digits.";
            }

            return null;
        }

        /// <summary>
        /// Validates that the email is in the correct format (Format: [username][@][domainname][.][topleveldomain]) 
        /// it also checks that the email does not exceed 50 characters.
        /// </summary>
        /// <param name="email">The email to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            if (email.Length > 50)
            {
                return "Email cannot exceed 50 characters.";
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Invalid email format.";
            }

            return null;
        }

        /// <summary>
        /// Validates that the last update date is present
        /// </summary>
        /// <param name="lastUpdateDate">The last update date to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateLastUpdateDate(DateTime lastUpdateDate)
        {
            if (lastUpdateDate == default)
            {
                return "Last update date is required.";
            }

            return null;
        }

        /// <summary>
        /// Validates that the contact Id is not a negative or 0. 
        /// </summary>
        /// <param name="contactID">The Id to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateContactID(int contactID)
        {
            if (contactID <= 0)
            {
                return "ContactID must be greater than 0.";
            }

            return null;
        }

        //Validation Methods:

        /// <summary>
        /// Validates a create contact request object. It validates the name, phone, 
        /// fax and email of the contact.
        /// </summary>
        /// <param name="request">The create contact request to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateCreateContact(CreateContactRequest request)
        {
            var error = ValidateName(request.Name);
            if (error != null)
                return error;

            error = ValidatePhone(request.Phone);
            if (error != null)
                return error;

            error = ValidateFax(request.Fax);
            if (error != null)
                return error;

            error = ValidateEmail(request.eMail);
            if (error != null)
                return error;

            return null;
        }


        /// <summary>
        /// Validates a update contact request object. It validates the id, name, 
        /// phone, name, fax and email.
        /// </summary>
        /// <param name="request">The update request to be validated</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateUpdateContact(UpdateContactRequest request)
        {
            var error = ValidateContactID(request.ContactID);
            if (error != null)
                return error;

            error = ValidatePhone(request.Phone);
            if (error != null)
                return error;

            error = ValidateName(request.Name);
            if (error != null)
                return error;

            error = ValidateFax(request.Fax);
            if (error != null)
                return error;

            error = ValidateEmail(request.eMail);
            if (error != null)
                return error;

            return null;

        }

        /// <summary>
        /// Validates a import contact request. It validates the name, phone, 
        /// fax, email, last update date.
        /// </summary>
        /// <param name="request">The import contact request to be validated.</param>
        /// <returns>The error string, null if passes validation</returns>
        public static string? ValidateImportedContact(ImportContactRequest request)
        {
            var error = ValidateName(request.Name);
            if (error != null)
                return error;

            error = ValidatePhone(request.Phone);
            if (error != null)
                return error;

            error = ValidateFax(request.Fax);
            if (error != null)
                return error;

            error = ValidateEmail(request.eMail);
            if (error != null)
                return error;

            error = ValidateLastUpdateDate(request.LastUpdateDate);
            if (error != null)
                return error;

            return null;
        }
    }
}