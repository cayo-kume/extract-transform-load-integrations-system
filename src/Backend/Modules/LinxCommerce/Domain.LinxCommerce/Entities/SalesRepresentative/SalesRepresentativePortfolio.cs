using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativePortfolio
    {
        public int? SalesRepresentativeID { get; set; }
        public bool? HasPortfolio { get; set; }
        public string? PortfolioAssociationType { get; set; }

        [SkipProperty]
        public List<SalesRepresentativeCustomerRelation>? Customers { get; set; } = new List<SalesRepresentativeCustomerRelation>();

        public SalesRepresentativePortfolio() { }

        public SalesRepresentativePortfolio(SalesRepresentativePortfolio salesRepresentativePortfolio, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.HasPortfolio = salesRepresentativePortfolio.HasPortfolio;
            this.PortfolioAssociationType = salesRepresentativePortfolio.PortfolioAssociationType;

            foreach (var customer in salesRepresentativePortfolio.Customers)
            {
                this.Customers.Add(new SalesRepresentativeCustomerRelation(customer, salesRepresentativeID));
            }
        }
    }
}
