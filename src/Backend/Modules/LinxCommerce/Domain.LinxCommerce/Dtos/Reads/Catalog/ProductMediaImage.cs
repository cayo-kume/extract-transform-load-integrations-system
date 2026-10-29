namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// ProductMediaImage do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class ProductMediaImage
    {
        public string? RelativePath { get; init; }
        public string? Extension { get; init; }
        public string? MaxWidth { get; init; }
        public string? MaxHeight { get; init; }
        public string? Width { get; init; }
        public string? Height { get; init; }
        public string? MediaSizeType { get; init; }
        public string? AbsolutePath { get; init; }
    }
}
