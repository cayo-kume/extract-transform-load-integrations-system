using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// cliente vinculado ao vendedor, tabela totvs.TotvsSellersCustomers
    /// </summary>
    public class Customer
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int sellerCode { get; private set; }
        public int personCode { get; private set; }
        public string document { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public Customer() { }

        public Customer(string doc_company, DateTime integrated_or_updated_at, int sellerCode, Dtos.Reads.Sellers.Customer dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.sellerCode = sellerCode;

            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.personCode);
            document = ConverterValidatorsExtensions.StringLengthValidation(dto.document, 14);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
        }
    }
}
