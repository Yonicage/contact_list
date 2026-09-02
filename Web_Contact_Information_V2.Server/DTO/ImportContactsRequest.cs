namespace Web_Contact_Information_V2.Server.DTO
{
    public class ImportContactsRequest
    {
        public required List<ImportContactRequest> Contacts { get; set; }

        public required string User { get; set; }

    }
}
