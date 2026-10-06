namespace Domain.LinxCommerce.Entities.Customer
{
    public class Contact
    {
        public DateTime lastupdateon { get; set; }

        public string? Phone { get; set; }

        public string? Phone2 { get; set; }

        public string? CellPhone { get; set; }

        public string? Fax { get; set; }

        public Int32? CustomerID { get; set; }

        public Contact() { }

        public Contact(Contact contact, Int32? customerID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.Phone = contact.Phone;
            this.Phone2 = contact.Phone2;
            this.CellPhone = contact.CellPhone;
            this.Fax = contact.Fax;
            this.CustomerID = customerID;
        }
    }
}
