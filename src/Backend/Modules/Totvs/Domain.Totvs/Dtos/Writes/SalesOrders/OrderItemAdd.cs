namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// item incluído (AddOrderItemsInDto): pelo productCode ou pelo productSku (o schema marca os dois como obrigatórios)
    /// </summary>
    public class OrderItemAdd
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public decimal? quantity { get; init; }
        public decimal? originalPrice { get; init; }
        public decimal? price { get; init; }
        public decimal? discountPercentage { get; init; }
        // facilitador: a Totvs calcula o percentual
        public decimal? discountValue { get; init; }
        public DateTime? billingForecastDate { get; init; }
        public DateTime? billingStartDate { get; init; }
        public int? customerOrderCode { get; init; }
        public string? customerProductCode { get; init; }
        public int? rollMeasureCode { get; init; }
        // um serial por unidade
        public List<string>? sequentials { get; init; }
        public List<OrderItemSequential>? sequentialsValues { get; init; }
        public List<OrderItemGroupObservation>? groupObservations { get; init; }
        public List<OrderItemCommissioned>? commissioneds { get; init; }
    }
}
