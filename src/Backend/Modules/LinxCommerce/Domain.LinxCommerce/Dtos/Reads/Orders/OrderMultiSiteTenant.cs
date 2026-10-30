namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderMultiSiteTenant do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderMultiSiteTenant
    {
        public string? BrandId { get; init; }
        public string? BrandType { get; init; }
        public string? CompanyId { get; init; }
        public string? DeviceType { get; init; }
    }
}
