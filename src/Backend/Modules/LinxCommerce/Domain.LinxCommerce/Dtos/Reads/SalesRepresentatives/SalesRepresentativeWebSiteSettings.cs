using Newtonsoft.Json.Linq;

namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeWebSiteSettings do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeWebSiteSettings
    {
        public string? WebSiteFilter { get; init; }

        public JToken? WebSiteGroups { get; init; }
        public JToken? WebSites { get; init; }
    }
}
