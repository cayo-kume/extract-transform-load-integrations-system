namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderDiscount do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderDiscount
    {
        public string? DiscountID { get; init; }
        public string? Amount { get; init; }
        public string? Message { get; init; }
        public string? Reference { get; init; }
        public string? Type { get; init; }
    }
}
