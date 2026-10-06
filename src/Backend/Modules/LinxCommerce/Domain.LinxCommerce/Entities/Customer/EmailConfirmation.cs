namespace Domain.LinxCommerce.Entities.Customer
{
    public class EmailConfirmation
    {
        public DateTime lastupdateon { get; set; }

        public Int32? CustomerID { get; set; }

        public string? Status { get; set; }

        public DateTimeOffset? ConfirmationDate { get; set; }

        public EmailConfirmation() { }

        public EmailConfirmation(EmailConfirmation emailConfirmation, Int32? customerID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.Status = emailConfirmation.Status;
            this.ConfirmationDate = emailConfirmation.ConfirmationDate;
            this.CustomerID = customerID;
        }
    }
}
