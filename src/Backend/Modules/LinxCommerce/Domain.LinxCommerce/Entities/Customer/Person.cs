using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Customer
{
    public class Person
    {
        public DateTime lastupdateon { get; set; }
        public string? Surname { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? RG { get; set; }
        public string? Cpf { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public Int32 CustomerID { get; set; }
        public Int32? CustomerStatusID { get; set; }
        public string? WebSiteID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Guid? CustomerHash { get; set; }
        public string? Password { get; set; }
        public Int32? CustomerType { get; set; }
        public string? Cnpj { get; set; }
        public string? TradingName { get; set; }
        public string? CustomerGroupID { get; set; }

        [SkipProperty]
        public List<Groups> Groups { get; set; } = new List<Groups>();

        [SkipProperty]
        public Contact Contact { get; set; }

        [SkipProperty]
        public List<PersonAddress> Address { get; set; } = new List<PersonAddress>();

        [SkipProperty]
        public EmailConfirmation EmailConfirmation { get; set; }

        [SkipProperty]
        public Dictionary<int, string> Responses { get; set; } = new Dictionary<int, string>();

        public Person() { }

        public Person(Person customer, string getCustomerResponse)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.Surname = customer.Surname;
            this.BirthDate = customer.BirthDate;
            this.Gender = customer.Gender;
            this.RG = customer.RG;
            this.Cpf = customer.Cpf;
            this.CreatedDate = customer.CreatedDate;
            this.CustomerID = customer.CustomerID;
            this.CustomerStatusID = customer.CustomerStatusID;
            this.WebSiteID = customer.WebSiteID;
            this.Name = customer.Name;
            this.Email = customer.Email;
            this.CustomerHash = customer.CustomerHash;
            this.Password = customer.Password;
            this.CustomerType = customer.CustomerType;
            this.Cnpj = customer.Cnpj;
            this.TradingName = customer.TradingName;
            this.CustomerGroupID = customer.CustomerGroupID;
            this.Responses.Add(customer.CustomerID, getCustomerResponse);

            this.Contact = new Contact(customer.Contact, customer.CustomerID);

            this.EmailConfirmation = new EmailConfirmation(customer.EmailConfirmation, customer.CustomerID);

            foreach (var group in customer.Groups)
            {
                this.Groups.Add(new Groups(group, customer.CustomerID));
            }

            foreach (var address in customer.Address)
            {
                this.Address.Add(new PersonAddress(address, customer.CustomerID));
            }
        }
    }
}
