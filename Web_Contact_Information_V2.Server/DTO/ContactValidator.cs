using System.Text.RegularExpressions;

namespace Web_Contact_Information_V2.Server.DTO
{
    public static class ContactValidator
    {
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

        public static string? ValidateLastUpdateDate(DateTime lastUpdateDate)
        {
            if (lastUpdateDate == default)
            {
                return "Last update date is required.";
            }

            return null;
        }

        public static string? ValidateContactID(int contactID)
        {
            if (contactID <= 0)
            {
                return "ContactID must be greater than 0.";
            }

            return null;
        }

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