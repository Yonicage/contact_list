namespace Web_Contact_Information_V2.Server.Model
{
    public class Contact
    {
        public Contact(int contactId, string name, string phone, string fax, string eMail, string notes, DateTime lastUpdateDate, string lastUpdateUserName)
        {
            ContactID = contactId;
            Name = name;
            Phone = phone;
            Fax = fax;
            this.eMail = eMail;
            Notes = notes;
            LastUpdateDate = lastUpdateDate;
            LastUpdateUserName = lastUpdateUserName;
        }
        public int ContactID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string? Fax { get; set; }
        public string? eMail { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUpdateUserName { get; set; }

    }
}
