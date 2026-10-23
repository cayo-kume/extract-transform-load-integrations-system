namespace Domain.Totvs.Dtos.Reads.PurchaseOrders
{
    /// <summary>
    /// item (produto) do pedido de compra
    /// </summary>
    public class Product
    {
        public string? productCode { get; init; }
        public string? name { get; init; }
        public string? referenceCode { get; init; }
        public string? referenceName { get; init; }
        public string? productSku { get; init; }
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public string? sizeName { get; init; }
        public string? measuredUnit { get; init; }
        public string? quantity { get; init; }
        public string? lowQuantity { get; init; }
        public string? settledQuantity { get; init; }
        public string? canceledQuantity { get; init; }
        public string? extraQuantity { get; init; }
        public string? deliveryForecastDate { get; init; }
        public string? deliveryDeadlineDate { get; init; }
        public string? value { get; init; }
        public string? discountPercentage { get; init; }
        public string? conversionUnit { get; init; }
        public string? conversionQuantity { get; init; }
    }
}
