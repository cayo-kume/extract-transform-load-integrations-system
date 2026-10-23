namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// desconto do pedido
    /// </summary>
    public class Discount
    {
        public string? typeDiscountCode { get; init; }
        public string? description { get; init; }
        public string? discountPercentage { get; init; }
        public string? discountValue { get; init; }
    }
}
