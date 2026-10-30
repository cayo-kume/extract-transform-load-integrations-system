namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderSeller do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderSeller
    {
        public string? EMail { get; init; }
        public string? IntegrationID { get; init; }
        public string? Name { get; init; }
        public string? Phone { get; init; }
        public string? SellerID { get; init; }
    }
}
