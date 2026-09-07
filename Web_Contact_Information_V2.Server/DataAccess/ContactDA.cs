/*
 * File: ContactsDA.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class handles the data access operations for contacts.
 */

using Microsoft.Data.SqlClient;
using System.Data;
using Web_Contact_Information_V2.Server.DTO;
using Web_Contact_Information_V2.Server.Model;

namespace Web_Contact_Information_V2.Server.DataAccess
{
    /// <summary>
    /// ContactDA: a class whose responsibility is to handle data 
    /// access operations for contacts. This class is responsible 
    /// for communicating directly with the SQL server.
    /// </summary>
    public class ContactDA
    {
        /// <summary>
        /// Stores the database connection string.
        /// </summary>
        private readonly string? _connectionString;

        /// <summary>
        /// A class that provides data access for a contact. When created, it gets the connect
        /// tion string from the configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings of the project.</param>
        public ContactDA(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// A method that provides all of the contacts present in the database.
        /// </summary>
        /// <returns>A list of contacts.</returns>
        public List<Contact> GetContacts()
        {
            var contacts = new List<Contact>();

            var statement =
                "SELECT ContactID, Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName " +
                "FROM Contacts";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                var contactID = (int)reader["ContactID"];
                var name = reader["Name"].ToString()!;
                var phone = reader["Phone"].ToString()!;
                string? fax = reader["Fax"].ToString();
                string? eMail = reader["eMail"].ToString();
                string? notes = reader["Notes"].ToString();
                var lastUpdateDate = (DateTime)reader["LastUpdateDate"];
                var lastUpdateUserName = reader["LastUpdateUserName"].ToString()!;
                contacts.Add(new Contact(contactID, name, phone, fax, eMail, notes, lastUpdateDate, lastUpdateUserName));
            }
            return contacts;

        }

        /// <summary>
        /// A method that provides all of the contacts in the database
        /// that partially or exactly match the provided name filter.
        /// </summary>
        /// <param name="nameFilter">The given filter to be used.</param>
        /// <returns>A filtered contact list.</returns>
        public List<Contact> FilterGetContact(string nameFilter)
        {
            var contacts = new List<Contact>();

            var statement =
                 "SELECT ContactID, Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName " +
                 "FROM Contacts " +
                 "WHERE Name LIKE @Name";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            command.Parameters.AddWithValue("@Name", "%" + nameFilter + "%"); //%: zero or more characters
            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                var contactID = (int)reader["ContactID"];
                var name = reader["Name"].ToString()!;
                var phone = reader["Phone"].ToString()!;
                string? fax = reader["Fax"].ToString();
                string? eMail = reader["eMail"].ToString();
                string? notes = reader["Notes"].ToString();
                var lastUpdateDate = (DateTime)reader["LastUpdateDate"];
                var lastUpdateUserName = reader["LastUpdateUserName"].ToString()!;
                contacts.Add(new Contact(contactID, name, phone, fax, eMail, notes, lastUpdateDate, lastUpdateUserName));
            }
            return contacts;
        }

        /// <summary>
        /// Adds a new contact to the contacts table.
        /// </summary>
        /// <param name="request">A create contact request, contains all values of the new contact.</param>
        /// <param name="username">The username of the user that created the request.</param>
        /// <returns>True if it was inserted into the database; otherwise, false.</returns>
        public bool AddContact(CreateContactRequest request)
        {
            var statement =
                "INSERT INTO Contacts(Name, Phone, Fax, eMail, Notes, LastUpdateUserName) " +
                "VALUES (@Name, @Phone, @Fax, @eMail, @Notes, @LastUpdateUserName)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Phone", request.Phone);
            command.Parameters.AddWithValue("@Fax", (request.Fax == null) ? DBNull.Value : request.Fax);
            command.Parameters.AddWithValue("@eMail", (request.eMail == null) ? DBNull.Value : request.eMail);
            command.Parameters.AddWithValue("@Notes", (request.Notes == null) ? DBNull.Value : request.Notes);
            command.Parameters.AddWithValue("@LastUpdateUserName", request.User);

            connection.Open();
            var rowCount = command.ExecuteNonQuery();
            return rowCount > 0;
        }

        /// <summary>
        /// Searches for a contact using its exact name and returns its contact Id.
        /// </summary>
        /// <param name="name">The contact name to be used to search for.</param>
        /// <returns>The contact Id number if found. If not found, it returns null.</returns>
        public int? GetContactIDByName(string name)
        {
            var statement =
                "SELECT ContactID " +
                "FROM Contacts " +
                "WHERE Name = @Name";
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(statement, connection);

            command.Parameters.AddWithValue("@Name", name);

            var result = command.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }
            return (int)result;
        }

        /// <summary>
        /// Imports a collection of contacts into the database.
        /// If a contact with the same name exists, its information is updated.
        /// If it does not exist a new contact is created.
        /// </summary>
        /// <returns>True when the import completes successfully.</returns>
        public bool ImportContacts(ImportContactsRequest request)
        {
            var contacts = request.Contacts; // list of imported contacts
            var username = request.User;     // Username of the user that requested the import 

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();

                foreach (var contact in contacts)
                {
                    var contactID = GetContactIDByName(contact.Name);

                    if (contactID != null) //If the contact exists, update. Else, create a new one. 
                    {
                        var statement =
                            "UPDATE Contacts " +
                            "SET Name = @Name, Phone = @Phone, Fax = @Fax, eMail = @eMail, Notes = @Notes, LastUpdateDate = @LastUpdateDate, LastUpdateUserName = @LastUpdateUserName " +
                            "WHERE ContactID = @ContactID";
                        using var command = new SqlCommand(statement, connection);

                        command.Parameters.AddWithValue("@ContactID", contactID);
                        command.Parameters.AddWithValue("@Name", contact.Name);
                        command.Parameters.AddWithValue("@Phone", contact.Phone);
                        command.Parameters.AddWithValue("@Fax", (contact.Fax == null) ? DBNull.Value : contact.Fax);
                        command.Parameters.AddWithValue("@eMail", (contact.eMail == null) ? DBNull.Value : contact.eMail);
                        command.Parameters.AddWithValue("@Notes", (contact.Notes == null) ? DBNull.Value : contact.Notes);
                        command.Parameters.AddWithValue("@LastUpdateDate", contact.LastUpdateDate);
                        command.Parameters.AddWithValue("@LastUpdateUserName", username);
                        command.ExecuteNonQuery();

                    }
                    else
                    {

                        var statement =
                             "INSERT INTO Contacts(Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName) " +
                             "VALUES (@Name, @Phone, @Fax, @eMail, @Notes, @LastUpdateDate, @LastUpdateUserName)";
                        using var command = new SqlCommand(statement, connection);
                        command.Parameters.AddWithValue("@Name", contact.Name);
                        command.Parameters.AddWithValue("@Phone", contact.Phone);
                        command.Parameters.AddWithValue("@Fax", (contact.Fax == null) ? DBNull.Value : contact.Fax);
                        command.Parameters.AddWithValue("@eMail", (contact.eMail == null) ? DBNull.Value : contact.eMail);
                        command.Parameters.AddWithValue("@Notes", (contact.Notes == null) ? DBNull.Value : contact.Notes);
                        command.Parameters.AddWithValue("@LastUpdateDate", contact.LastUpdateDate);
                        command.Parameters.AddWithValue("@LastUpdateUserName", username);
                        command.ExecuteNonQuery();
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DATABASE ERROR:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Updates the existing contact using its contact Id
        /// </summary>
        /// <param name="request">The contact requested to be updated </param>
        /// <returns>True if contact was successfully updated; otherwise, false.</returns>
        public bool UpdateContact(UpdateContactRequest request)
        {
            var statement =
                "UPDATE Contacts " +
                "SET Name = @NewName, Phone = @NewPhone, Fax = @NewFax, eMail = @NeweMail, Notes = @NewNotes, LastUpdateDate = GETDATE(), LastUpdateUserName = @NewLastUpdateUserName " +
                "WHERE ContactID = @ContactID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            command.Parameters.AddWithValue("@ContactID", request.ContactID);
            command.Parameters.AddWithValue("@NewName", request.Name);
            command.Parameters.AddWithValue("@NewPhone", request.Phone);
            command.Parameters.AddWithValue("@NewFax", (request.Fax == null) ? DBNull.Value : request.Fax);
            command.Parameters.AddWithValue("@NeweMail", (request.eMail == null) ? DBNull.Value : request.eMail);
            command.Parameters.AddWithValue("@NewNotes", (request.Notes == null) ? DBNull.Value : request.Notes);
            command.Parameters.AddWithValue("@NewLastUpdateUserName", request.User);

            connection.Open();
            var rowCount = command.ExecuteNonQuery();
            return rowCount > 0;
        }

        /// <summary>
        /// Resets all Contacts in the database.
        /// It deletes all the contacts and recreates them using a given 
        /// contact list. The original Contact Id values are preserved.
        /// </summary>
        /// <param name="contacts">The contact list that will rerplace the database contents</param>
        /// <returns>True when the reset operation completes successfully.</returns>
        public bool ResetContacts(List<Contact> contacts)
        {
            var statement =
              "DELETE FROM Contacts; " +
              "DBCC CHECKIDENT ('Contacts', RESEED, 0); " +
              "SET IDENTITY_INSERT Contacts ON;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();


                foreach (var contact in contacts)
                {
                    var insertStatement =
                        "INSERT INTO Contacts(ContactID, Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName) " +
                        "VALUES (@ContactID, @Name, @Phone, @Fax, @eMail, @Notes, @LastUpdateDate, @LastUpdateUserName)";
                    using var insertCommand = new SqlCommand(insertStatement, connection);

                    insertCommand.Parameters.AddWithValue("@ContactID", contact.ContactID);
                    insertCommand.Parameters.AddWithValue("@Name", contact.Name);
                    insertCommand.Parameters.AddWithValue("@Phone", contact.Phone);
                    insertCommand.Parameters.AddWithValue("@Fax", (contact.Fax == null) ? DBNull.Value : contact.Fax);
                    insertCommand.Parameters.AddWithValue("@eMail", (contact.eMail == null) ? DBNull.Value : contact.eMail);
                    insertCommand.Parameters.AddWithValue("@Notes", (contact.Notes == null) ? DBNull.Value : contact.Notes);
                    insertCommand.Parameters.AddWithValue("@LastUpdateDate", contact.LastUpdateDate);
                    insertCommand.Parameters.AddWithValue("@LastUpdateUserName", contact.LastUpdateUserName);

                    insertCommand.ExecuteNonQuery();

                }
                var offStatement =
                     "SET IDENTITY_INSERT Contacts OFF;";
                using var offCommand = new SqlCommand(offStatement, connection);
                offCommand.ExecuteNonQuery();
                return true;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
                throw;
            }
        }
    }
}
