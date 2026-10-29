namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// ProductMediaVideo do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class ProductMediaVideo
    {
        public string? Title { get; init; }
        public string? Url { get; init; }
    }
}
