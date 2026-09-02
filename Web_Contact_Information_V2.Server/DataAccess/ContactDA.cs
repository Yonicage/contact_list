using Microsoft.Data.SqlClient;
using System.Data;
using Web_Contact_Information_V2.Server.DTO;
using Web_Contact_Information_V2.Server.Model;

namespace Web_Contact_Information_V2.Server.DataAccess
{
    /// <summary>
    /// ContactDA: a class whose responsibility is to access and manage data.
    /// </summary>
    public class ContactDA
    {

        private readonly string? _connectionString;

        public ContactDA(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); //OJO string? puede ser null //do we need this, part of the dependency Injection?
        }
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
        public List<Contact> FilterGetContact(string contactNameIn)
        {
            var contacts = new List<Contact>();
            var statement =
                 "SELECT ContactID, Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName " +
                 "FROM Contacts " +
                 "WHERE Name LIKE @Name";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(statement, connection);

            command.Parameters.AddWithValue("@Name", "%" + contactNameIn + "%");
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
                contacts.Add(new Contact(contactID, name, phone, fax, eMail, notes, lastUpdateDate, lastUpdateUserName)); ///
            }
            return contacts;
        }
        public bool AddContact(CreateContactRequest request, string username)           // addwith value problem in terms of value (x) Change with explicit SQL? !!!!!!!!!!!!!!!!!!!!!!
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

        public bool ImportContacts(List<ImportContactRequest> requests, string username)
        {

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();


                foreach (var request in requests)
                {

                    var contactID = GetContactIDByName(request.Name);
                    Console.WriteLine($"ContactID = {contactID}");
                    var statement = "";


                    if (contactID != null)
                    {
                        statement = "" +
                            "UPDATE Contacts " +
                            "SET Name = @Name, Phone = @Phone, Fax = @Fax, eMail = @eMail, Notes = @Notes, LastUpdateDate = @LastUpdateDate, LastUpdateUserName = @LastUpdateUserName " +
                            "WHERE ContactID = @ContactID";
                        using var command = new SqlCommand(statement, connection);

                        command.Parameters.AddWithValue("@ContactID", contactID);
                        command.Parameters.AddWithValue("@Name", request.Name);
                        command.Parameters.AddWithValue("@Phone", request.Phone);
                        command.Parameters.AddWithValue("@Fax", (request.Fax == null) ? DBNull.Value : request.Fax);
                        command.Parameters.AddWithValue("@eMail", (request.eMail == null) ? DBNull.Value : request.eMail);
                        command.Parameters.AddWithValue("@Notes", (request.Notes == null) ? DBNull.Value : request.Notes);
                        command.Parameters.AddWithValue("@LastUpdateDate", request.LastUpdateDate);
                        command.Parameters.AddWithValue("@LastUpdateUserName", username);
                        command.ExecuteNonQuery();

                    }
                    else
                    {

                        statement =
                            "INSERT INTO Contacts(Name, Phone, Fax, eMail, Notes, LastUpdateDate, LastUpdateUserName) " +
                            "VALUES (@Name, @Phone, @Fax, @eMail, @Notes, @LastUpdateDate, @LastUpdateUserName)";
                        using var command = new SqlCommand(statement, connection);
                        command.Parameters.AddWithValue("@Name", request.Name);
                        command.Parameters.AddWithValue("@Phone", request.Phone);
                        command.Parameters.AddWithValue("@Fax", (request.Fax == null) ? DBNull.Value : request.Fax);
                        command.Parameters.AddWithValue("@eMail", (request.eMail == null) ? DBNull.Value : request.eMail);
                        command.Parameters.AddWithValue("@Notes", (request.Notes == null) ? DBNull.Value : request.Notes);
                        command.Parameters.AddWithValue("@LastUpdateDate", request.LastUpdateDate);
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
                throw ex;


            }

        }


        public bool UpdateContact(UpdateContactRequest request, string username)
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
            command.Parameters.AddWithValue("@NewLastUpdateUserName", username);

            connection.Open();
            var rowCount = command.ExecuteNonQuery();
            return rowCount > 0;
        }

        //TODO: Add delete when all works
    }
}
