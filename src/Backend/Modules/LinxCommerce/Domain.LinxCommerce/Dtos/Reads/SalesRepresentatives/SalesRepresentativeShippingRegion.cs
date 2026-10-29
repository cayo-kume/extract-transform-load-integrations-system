using Newtonsoft.Json.Linq;

namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeShippingRegion do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeShippingRegion
    {
        public string? SelectedMode { get; init; }
        public string? ShippingRegionID { get; init; }

        public JToken? PointOfSalesList { get; init; }
    }
}
