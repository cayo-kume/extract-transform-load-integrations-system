using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentative
    {
        public int? SalesRepresentativeID { get; set; }
        public string? Name { get; set; }
        public string? FriendlyCode { get; set; }
        public string? ImageUrl { get; set; }
        public string? IntegrationID { get; set; }
        public bool? AllowQuoteDeletion { get; set; }
        public int? BusinessContractID { get; set; }
        public string? SalesRepresentativeType { get; set; }
        public string? Status { get; set; }

        [SkipProperty]
        public List<int>? OrderTypeItems { get; set; }

        [SkipProperty]
        public Dictionary<int?, string> Responses { get; set; } = new Dictionary<int?, string>();

        [SkipProperty]
        public SalesRepresentativeIdentification? Identification { get; set; }

        [SkipProperty]
        public SalesRepresentativeComission? Commission { get; set; }

        [SkipProperty]
        public SalesRepresentativeContactData? Contact { get; set; }

        [SkipProperty]
        public SalesRepresentativeShippingRegion? ShippingRegion { get; set; }

        [SkipProperty]
        public List<SalesRepresentativeAddress> Addresses { get; set; } = new List<SalesRepresentativeAddress>();

        [SkipProperty]
        public SalesRepresentativeMaxDiscount? MaxDiscount { get; set; }

        [SkipProperty]
        public SalesRepresentativeWebSiteSettings? WebSiteSettings { get; set; }

        [SkipProperty]
        public SalesRepresentativePortfolio? Portfolio { get; set; }

        public SalesRepresentative() { }

        public SalesRepresentative(SalesRepresentative salesRepresentative)
        {
            this.SalesRepresentativeID = salesRepresentative.SalesRepresentativeID == null ? 0 : salesRepresentative.SalesRepresentativeID;
            this.Name = salesRepresentative.Name;
            this.FriendlyCode = salesRepresentative.FriendlyCode;
            this.ImageUrl = salesRepresentative.ImageUrl;
            this.IntegrationID = salesRepresentative.IntegrationID;
            this.AllowQuoteDeletion = salesRepresentative.AllowQuoteDeletion;
            this.BusinessContractID = salesRepresentative.BusinessContractID;
            this.SalesRepresentativeType = salesRepresentative.SalesRepresentativeType;
            this.Status = salesRepresentative.Status;

            this.Identification = salesRepresentative.Identification;
            this.Commission = salesRepresentative.Commission;
            this.Contact = salesRepresentative.Contact;
            this.ShippingRegion = salesRepresentative.ShippingRegion;
            this.MaxDiscount = salesRepresentative.MaxDiscount;
            this.WebSiteSettings = salesRepresentative.WebSiteSettings;
            this.Portfolio = salesRepresentative.Portfolio;

            foreach (var address in salesRepresentative.Addresses)
            {
                this.Addresses.Add(new SalesRepresentativeAddress(address));
            }
        }

        public SalesRepresentative(SalesRepresentative salesRepresentative, string getSaleRepresentativeResponse)
        {
            this.SalesRepresentativeID = salesRepresentative.SalesRepresentativeID;
            this.Name = salesRepresentative.Name;
            this.FriendlyCode = salesRepresentative.FriendlyCode;
            this.ImageUrl = salesRepresentative.ImageUrl;
            this.IntegrationID = salesRepresentative.IntegrationID;
            this.AllowQuoteDeletion = salesRepresentative.AllowQuoteDeletion;
            this.BusinessContractID = salesRepresentative.BusinessContractID;
            this.SalesRepresentativeType = salesRepresentative.SalesRepresentativeType;
            this.Status = salesRepresentative.Status;

            this.Identification = salesRepresentative.Identification;
            this.Commission = salesRepresentative.Commission;
            this.Contact = salesRepresentative.Contact;
            this.ShippingRegion = salesRepresentative.ShippingRegion;
            this.MaxDiscount = salesRepresentative.MaxDiscount;
            this.WebSiteSettings = salesRepresentative.WebSiteSettings;
            this.Portfolio = salesRepresentative.Portfolio;

            this.Responses.Add(salesRepresentative.SalesRepresentativeID, getSaleRepresentativeResponse);

            foreach (var address in salesRepresentative.Addresses)
            {
                this.Addresses.Add(new SalesRepresentativeAddress(address));
            }
        }
    }
}
