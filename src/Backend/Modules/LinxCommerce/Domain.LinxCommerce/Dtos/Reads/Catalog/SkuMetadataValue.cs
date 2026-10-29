namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// SkuMetadataValue do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SkuMetadataValue
    {
        public string? PropertyMetadataID { get; init; }
        public string? PropertyName { get; init; }
        public string? PropertyGroup { get; init; }
        public string? InputType { get; init; }
        public string? Value { get; init; }
        public string? SerializedValue { get; init; }
        public string? SerializedBlobValue { get; init; }
        public string? IntegrationID { get; init; }
        public string? FormattedValue { get; init; }
        public string? DisplayName { get; init; }
    }
}
