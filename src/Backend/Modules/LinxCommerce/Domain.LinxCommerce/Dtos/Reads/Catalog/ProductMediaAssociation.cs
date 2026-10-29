namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// ProductMediaAssociation do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class ProductMediaAssociation
    {
        public string? ID { get; init; }
        public string? MediaID { get; init; }
        public string? Path { get; init; }
        public string? Order { get; init; }
    }
}
