using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Customers
{
    /// <summary>
    /// grupo do cliente (hoje gravado como texto em Customer.CustomerGroupID), tabela linxcommerce.LinxCommerceCustomersGroups
    /// </summary>
    public class CustomerGroup
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int CustomerID { get; private set; }
        public string CustomerGroupID { get; private set; } = String.Empty;
        public string Title { get; private set; } = String.Empty;

        public CustomerGroup() { }

        public CustomerGroup(string doc_company, DateTime integrated_or_updated_at, int CustomerID, Dtos.Reads.Customers.CustomerGroup dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.CustomerID = CustomerID;

            CustomerGroupID = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerGroupID, 50);
            Title = ConverterValidatorsExtensions.StringLengthValidation(dto.Title, 250);
        }
    }
}
