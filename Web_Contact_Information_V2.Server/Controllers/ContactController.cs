using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using Web_Contact_Information_V2.Server.DataAccess;
using Web_Contact_Information_V2.Server.DTO;
using Web_Contact_Information_V2.Server.Model;

namespace Web_Contact_Information_V2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactDA _contactDA;

        private readonly IWebHostEnvironment _environment;

        public ContactController(ContactDA contactDA, IWebHostEnvironment environment)
        {
            _contactDA = contactDA;
            _environment = environment;

        }

        [HttpGet]
        public ActionResult<List<Contact>> Get(string? filter) //this method can be anything  ActionResult gives collection list, 200, 404, 400 error ,Ienumerable only shows collection 
        {

            try
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    var contactList = _contactDA.GetContacts();
                    return contactList;


                }
                else
                {
                    var contactList = _contactDA.FilterGetContact(filter);
                    return contactList;
                }

            }
            catch (SqlException)
            {
                return StatusCode(500, "A database error occurred.");
            }
        }
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

                var result = _contactDA.AddContact(request, request.User);
                if (result)
                {
                    return Ok(true);
                }
                return BadRequest("Contact could not be added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
                return false;
            }
        }

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
                var result = _contactDA.ImportContacts(request.Contacts, request.User);
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


                var result = _contactDA.UpdateContact(request, request.User);

                if (result)
                {
                    return Ok(true);
                }
                return NotFound("Contact was not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");

            }
            return false;
        }

        [HttpPost("reset")]
        public ActionResult<bool> PostRESET()
        {
            string FilePath = Path.Combine(
                _environment.ContentRootPath, "Seed", "seed.json"
                );


            var jsonString = System.IO.File.ReadAllText(FilePath);
            var contacts = JsonSerializer.Deserialize<List<Contact>>(jsonString)!; //May be null 
            try
            {


                return _contactDA.ResetContacts(contacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database operation failed: {ex.Message}");
                return false;
            }
        }







    }


}
