namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// item (produto) do pedido de venda
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
        public string? quantity { get; init; }
        public string? toSettleQuantity { get; init; }
        public string? settledQuantity { get; init; }
        public string? canceledQuantity { get; init; }
        public string? extraQuantity { get; init; }
        public string? pendingQuantity { get; init; }
        public string? originalPrice { get; init; }
        public string? price { get; init; }
        public string? discountPercentage { get; init; }
        public string? lastChangeUserInsert { get; init; }
        public string? lastChangeDate { get; init; }
        public string? billingForecastDate { get; init; }
        public string? insertDate { get; init; }
        public string? cancellationLimitDate { get; init; }
        public string? billingLimitDate { get; init; }
        public string? lastBillingDate { get; init; }
        public string? rollMeasureCode { get; init; }
        public string? rollMeasureDescription { get; init; }
        public string? customerOrderCode { get; init; }
        public string? customerProductCode { get; init; }

        public List<BatchItem>? batchItems { get; init; }
    }
}
