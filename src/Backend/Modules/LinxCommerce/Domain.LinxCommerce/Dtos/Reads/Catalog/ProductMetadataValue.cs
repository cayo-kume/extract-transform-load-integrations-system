namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// ProductMetadataValue do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class ProductMetadataValue
    {
        public string? PropertyMetadataID { get; init; }
        public string? PropertyName { get; init; }
        public string? DisplayName { get; init; }
        public string? FormattedValue { get; init; }
        public string? PropertyGroup { get; init; }
    }
}
