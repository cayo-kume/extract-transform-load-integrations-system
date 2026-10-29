using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.SalesRepresentatives
{
    /// <summary>
    /// cliente da carteira do vendedor (Portfolio.Customers), tabela linxcommerce.LinxCommerceSalesRepresentativesCustomers
    /// </summary>
    public class SalesRepresentativeCustomer
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int SalesRepresentativeID { get; private set; }
        public string CustomerID { get; private set; } = String.Empty;
        public string Status { get; private set; } = String.Empty;
        public string IsMaxDiscountEnabled { get; private set; } = String.Empty;

        public SalesRepresentativeCustomer() { }

        public SalesRepresentativeCustomer(string doc_company, DateTime integrated_or_updated_at, int SalesRepresentativeID, Dtos.Reads.SalesRepresentatives.SalesRepresentativeCustomer dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.SalesRepresentativeID = SalesRepresentativeID;

            CustomerID = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerID, 50);
            Status = ConverterValidatorsExtensions.StringLengthValidation(dto.Status, 50);
            IsMaxDiscountEnabled = ConverterValidatorsExtensions.StringLengthValidation(dto.IsMaxDiscountEnabled, 10);
        }
    }
}
