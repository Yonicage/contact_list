/*
 * File: ContactController.cs
 * Author: Yoniel Ruiz Alfaro
 * Date: 09/02/2026
 * Purpose: This class is a controller for managing 
 * contact-related HTTP requests.
 */

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web_Contact_Information_V2.Server.DataAccess;
using Web_Contact_Information_V2.Server.DTO;
using Web_Contact_Information_V2.Server.Model;

namespace Web_Contact_Information_V2.Server.Controllers
{
    /// <summary>
    /// Controller responsible for handling HTTP requests related to contacts.
    /// It receives requests from the client, validates the data, and uses 
    /// contactDA to perform database operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        /// <summary>
        /// Provides access to the contact database operation
        /// </summary>
        private readonly ContactDA _contactDA;

        /// <summary>
        /// Provides the application's hosting environment
        /// In this case we use it for the application's root directory.
        /// </summary>
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes the ContactController with the required dependancies.
        /// </summary>
        /// <param name="contactDA">The data-access class used to manage contacts</param>
        /// <param name="environment">The application's hosting environment.</param>
        public ContactController(ContactDA contactDA, IWebHostEnvironment environment)
        {
            _contactDA = contactDA;
            _environment = environment;

        }

        /// <summary>
        /// Retrieves all contacts.
        /// </summary>
        /// <returns>A list of contacts, or an HTTP 500 response if server error occurs.</returns>
        [HttpGet]
        public ActionResult<List<Contact>> Get(string? filter)
        {
            try
            {
                    var contactList = _contactDA.GetContacts();
                    return contactList;

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all contacts or filters contacts by name when a filter is provided.
        /// </summary>
        /// <param name="filter">An optional name filter used to search for contacts.</param>
        /// <returns>A list of contacts, or an HTTP 500 response if server error occurs.</returns>
        [HttpGet]
        public ActionResult<List<Contact>> GetFilter(string? filter)
        {

            try
            {
                    var contactList = _contactDA.FilterGetContact(filter);
                    return contactList;
               

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Inserts a new contact from a request after validating its data.
        /// </summary>
        /// <param name="request">A create contact request made by the user</param>
        /// <returns>
        /// HTTP 200 if contact is created succsessfully,
        /// HTTP 400 if validation or creation fails,
        /// HTTP 500 if a server error occurs.
        /// </returns>
        [HttpPost]
        public ActionResult<bool> Post([FromBody] CreateContactRequest request) //model binding
        {
            var validationError = ContactValidator.ValidateCreateContact(request);

            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {

                var result = _contactDA.AddContact(request);
                if (result)
                {
                    return Ok(true);
                }
                return BadRequest("Contact could not be added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Imports the contacts list from the request, it also validates each contact
        /// from the list.
        /// </summary>
        /// <param name="request">The import request made from the user,
        /// it contains the contact list and username  of the user.
        /// </param>
        /// <returns>
        /// HTTP 200 if the import was successfull,
        /// HTTP 400 if the validation or import fails,
        /// HTTP 500 if a server error occurs.
        /// </returns>
        [HttpPost("import")]
        public ActionResult<bool> PostImport([FromBody] ImportContactsRequest request)
        {

            foreach (var contact in request.Contacts)
            {
                var validationError = ContactValidator.ValidateImportedContact(contact);
                if (validationError != null)
                {
                    return BadRequest(validationError);
                }
            }


            try
            {
                var result = _contactDA.ImportContacts(request);
                if (result)
                {
                    return Ok(true);
                }
                return BadRequest("Contacts could not be imported.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CONTROLLER ERROR:");
                Console.WriteLine(ex.ToString());

                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Updates the selected contact from the request after passing validation checks.
        /// </summary>
        /// <param name="request">The contact to be updated.</param>
        /// <returns>
        /// HTTP 200 if updated succsessfully
        /// HTTP 400 if update fails
        /// HTTP 404 if contact is not found
        /// HTTP 500 if a server error occurs.
        /// </returns>
        [HttpPut]
        public ActionResult<bool> Put([FromBody] UpdateContactRequest request)
        {
            var validationError = ContactValidator.ValidateUpdateContact(request);

            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                var result = _contactDA.UpdateContact(request);

                if (result)
                {
                    return Ok(true);
                }
                return NotFound("Contact was not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
                return StatusCode(500, ex.Message);
            }

        }


        /// <summary>
        /// Resets the contacts database using the contacts stored in the seed JSON file.
        /// </summary>
        /// <returns>True, if succsessfull; otherwise, false.</returns>
        [HttpPost("reset")]
        public ActionResult<bool> PostRESET()
        {
            string FilePath = Path.Combine(
                _environment.ContentRootPath, "SeedData", "seed.json"
                );


            var jsonString = System.IO.File.ReadAllText(FilePath);
            var contacts = JsonSerializer.Deserialize<List<Contact>>(jsonString)!;

            return _contactDA.ResetContacts(contacts);
        }

    }


}

