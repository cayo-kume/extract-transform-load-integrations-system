using Newtonsoft.Json.Linq;

namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentative do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentative
    {
        public string? SalesRepresentativeID { get; init; }
        public string? Name { get; init; }
        public string? FriendlyCode { get; init; }
        public string? ImageUrl { get; init; }
        public string? IntegrationID { get; init; }
        public string? AllowQuoteDeletion { get; init; }
        public string? BusinessContractID { get; init; }
        public string? SalesRepresentativeType { get; init; }
        public string? Status { get; init; }

        public JToken? OrderTypeItems { get; init; }
        public SalesRepresentativeIdentification? Identification { get; init; }
        public SalesRepresentativeCommission? Commission { get; init; }
        public SalesRepresentativeContact? Contact { get; init; }
        public SalesRepresentativeShippingRegion? ShippingRegion { get; init; }
        public SalesRepresentativeMaxDiscount? MaxDiscount { get; init; }
        public SalesRepresentativeWebSiteSettings? WebSiteSettings { get; init; }
        public SalesRepresentativePortfolio? Portfolio { get; init; }
        public List<SalesRepresentativeAddress>? Addresses { get; init; }
    }
}
