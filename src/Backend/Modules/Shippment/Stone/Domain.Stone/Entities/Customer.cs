using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Customer
    {
        public string name { get; set; }
        public string email { get; set; }
        public string document { get; set; } = "0";
        public string phoneNumber { get; set; }

        public Customer() { }

        public Customer(Dtos.Customer customer)
        {
            this.name = ConverterValidatorsExtensions.StringLengthValidation(customer.name, 60);
            this.email = ConverterValidatorsExtensions.StringLengthValidation(customer.email, 50);
            this.document = ConverterValidatorsExtensions.StringLengthValidation(customer.document, 14);
            this.phoneNumber = ConverterValidatorsExtensions.StringLengthValidation(customer.phoneNumber, 20);
        }
    }
}
