using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Customers
{
    /// <summary>
    /// endereço do cliente, tabela linxcommerce.LinxCommerceCustomersAddresses
    /// </summary>
    public class CustomerAddress
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int CustomerID { get; private set; }
        public int ID { get; private set; }
        public bool? MainAddress { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string ContactName { get; private set; } = String.Empty;
        public string AddressLine { get; private set; } = String.Empty;
        public string Number { get; private set; } = String.Empty;
        public string Neighbourhood { get; private set; } = String.Empty;
        public string City { get; private set; } = String.Empty;
        public string State { get; private set; } = String.Empty;
        public string PostalCode { get; private set; } = String.Empty;
        public string AddressNotes { get; private set; } = String.Empty;
        public string Landmark { get; private set; } = String.Empty;

        public CustomerAddress() { }

        public CustomerAddress(string doc_company, DateTime integrated_or_updated_at, int CustomerID, Dtos.Reads.Customers.CustomerAddress dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.CustomerID = CustomerID;

            ID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.ID);
            MainAddress = NullableConverters.ToNullableBoolean(dto.MainAddress);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            ContactName = ConverterValidatorsExtensions.StringLengthValidation(dto.ContactName, 250);
            AddressLine = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressLine, 400);
            Number = ConverterValidatorsExtensions.StringLengthValidation(dto.Number, 50);
            Neighbourhood = ConverterValidatorsExtensions.StringLengthValidation(dto.Neighbourhood, 250);
            City = ConverterValidatorsExtensions.StringLengthValidation(dto.City, 250);
            State = ConverterValidatorsExtensions.StringLengthValidation(dto.State, 50);
            PostalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.PostalCode, 20);
            AddressNotes = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressNotes, 400);
            Landmark = ConverterValidatorsExtensions.StringLengthValidation(dto.Landmark, 400);
        }
    }
}
