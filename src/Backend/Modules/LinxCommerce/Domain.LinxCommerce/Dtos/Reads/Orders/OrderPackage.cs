namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderPackage do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderPackage
    {
        public string? OrderPackageID { get; init; }
        public string? DeliveryMethodID { get; init; }
        public string? PackageNumber { get; init; }
        public string? TrackingNumber { get; init; }
        public string? TrackingNumberUrl { get; init; }
        public string? ShippedDate { get; init; }
        public string? ShippedBy { get; init; }
        public string? IsDeleted { get; init; }
        public string? PackageType { get; init; }
        public string? Source { get; init; }
        public string? InsuranceAmount { get; init; }
        public string? Height { get; init; }
        public string? Width { get; init; }
        public string? Length { get; init; }
        public string? Weight { get; init; }
    }
}
