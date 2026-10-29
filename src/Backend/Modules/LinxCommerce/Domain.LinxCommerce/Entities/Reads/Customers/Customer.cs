using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Customers
{
    /// <summary>
    /// cliente (POST Profile/API.svc/web/GetPerson ou GetCompany), com Contact e EmailConfirmation (1:1) em colunas e sem senha nem hash, tabela linxcommerce.LinxCommerceCustomers
    /// </summary>
    public class Customer
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int CustomerID { get; private set; }
        public string CustomerType { get; private set; } = String.Empty;
        public int? CustomerStatusID { get; private set; }
        public string WebSiteID { get; private set; } = String.Empty;
        public string Name { get; private set; } = String.Empty;
        public string Surname { get; private set; } = String.Empty;
        public string TradingName { get; private set; } = String.Empty;
        public string Email { get; private set; } = String.Empty;
        public string Cpf { get; private set; } = String.Empty;
        public string Cnpj { get; private set; } = String.Empty;
        public string RG { get; private set; } = String.Empty;
        public string Gender { get; private set; } = String.Empty;
        public DateTimeOffset? BirthDate { get; private set; }
        public DateTimeOffset? CreatedDate { get; private set; }
        public string ContactPhone { get; private set; } = String.Empty;
        public string ContactPhone2 { get; private set; } = String.Empty;
        public string ContactCellPhone { get; private set; } = String.Empty;
        public string ContactFax { get; private set; } = String.Empty;
        public string EmailConfirmationStatus { get; private set; } = String.Empty;
        public DateTimeOffset? EmailConfirmationConfirmationDate { get; private set; }

        [SkipProperty]
        public List<CustomerAddress> addresses { get; private set; } = [];

        [SkipProperty]
        public List<CustomerGroup> groups { get; private set; } = [];

        public Customer() { }

        public Customer(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Customers.Customer dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            CustomerID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.CustomerID);
            CustomerType = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerType, 10);
            CustomerStatusID = NullableConverters.ToNullableInt32(dto.CustomerStatusID);
            WebSiteID = ConverterValidatorsExtensions.StringLengthValidation(dto.WebSiteID, 50);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            Surname = ConverterValidatorsExtensions.StringLengthValidation(dto.Surname, 250);
            TradingName = ConverterValidatorsExtensions.StringLengthValidation(dto.TradingName, 250);
            Email = ConverterValidatorsExtensions.StringLengthValidation(dto.Email, 250);
            Cpf = ConverterValidatorsExtensions.StringLengthValidation(dto.Cpf, 20);
            Cnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.Cnpj, 20);
            RG = ConverterValidatorsExtensions.StringLengthValidation(dto.RG, 30);
            Gender = ConverterValidatorsExtensions.StringLengthValidation(dto.Gender, 10);
            BirthDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.BirthDate);
            CreatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedDate);
            ContactPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.Phone, 50);
            ContactPhone2 = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.Phone2, 50);
            ContactCellPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.CellPhone, 50);
            ContactFax = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.Fax, 50);
            EmailConfirmationStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.EmailConfirmation?.Status, 50);
            EmailConfirmationConfirmationDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.EmailConfirmation?.ConfirmationDate);

            // repetidos violariam a PK; sem a chave a linha também violaria
            foreach (var item in (dto.Address ?? []).Select(x => new CustomerAddress(doc_company, integrated_or_updated_at, CustomerID, x)).Where(x => x.ID != 0).GroupBy(x => x.ID).Select(g => g.Last()))
                addresses.Add(item);
            foreach (var item in (dto.Groups ?? []).Select(x => new CustomerGroup(doc_company, integrated_or_updated_at, CustomerID, x)).Where(x => !String.IsNullOrEmpty(x.CustomerGroupID)).GroupBy(x => x.CustomerGroupID).Select(g => g.Last()))
                groups.Add(item);
        }
    }
}
