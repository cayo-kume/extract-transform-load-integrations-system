using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Seller
{
    public class Customer
    {
        public int sellerCode { get; private set; }

        public int personCode { get; private set; }
        
        public string document { get; private set; }
        public string name { get; private set; }

        public Customer() { }

        public Customer(Domain.Totvs.Dtos.Seller.Customer customerDto, int sellerCode)
        {
            this.sellerCode = sellerCode;

            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.personCode);

            document = customerDto.document?.Length > 14 ? customerDto.document?.Substring(0, 14) : customerDto.document;
            name = customerDto.name?.Length > 250 ? customerDto.name?.Substring(0, 250) : customerDto.name;
        }
    }
}
