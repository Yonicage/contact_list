using System.ComponentModel.DataAnnotations;

namespace Web_Contact_Information_V2.Server.DTO
{
    public class ImportContactRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        public string? Fax { get; set; }
        public string? eMail { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUpdateDate { get; set; }


    }
}
