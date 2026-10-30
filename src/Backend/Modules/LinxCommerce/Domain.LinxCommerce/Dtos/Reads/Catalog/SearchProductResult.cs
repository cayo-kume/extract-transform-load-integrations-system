namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// SearchProductResult do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SearchProductResult
    {
        public string? ProductID { get; init; }
        public string? Name { get; init; }
        public string? SKU { get; init; }
        public string? IntegrationID { get; init; }
        public string? CreatedDate { get; init; }
        public string? ModifiedDate { get; init; }
    }
}
