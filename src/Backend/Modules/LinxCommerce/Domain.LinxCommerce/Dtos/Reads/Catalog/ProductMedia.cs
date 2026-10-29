namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// ProductMedia do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class ProductMedia
    {
        public string? MediaID { get; init; }
        public string? IsDeleted { get; init; }
        public string? Index { get; init; }
        public string? CreatedDate { get; init; }
        public string? Type { get; init; }
        public string? ParentMediaID { get; init; }
        public string? OriginalFileName { get; init; }

        public ProductMediaImage? Image { get; init; }
        public ProductMediaVideo? Video { get; init; }
    }
}
