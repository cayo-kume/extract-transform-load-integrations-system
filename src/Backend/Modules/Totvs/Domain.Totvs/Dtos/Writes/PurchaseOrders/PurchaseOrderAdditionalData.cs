namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// informações adicionais do pedido de compra (PurchaseOrderAdditionalDataModel); o swagger não dá tamanhos
    /// </summary>
    public class PurchaseOrderAdditionalData
    {
        public int? currencyCode { get; init; }
        public decimal? currencyRateValue { get; init; }
        public int? costCenter { get; init; }
        public decimal? maximumExtraQuantityPercentage { get; init; }
        public decimal? maximumExtraQuantity { get; init; }
        public OrderSettleType? settleType { get; init; }
        public decimal? accessoryExpensePercentage { get; init; }
        public decimal? freightPercentage { get; init; }
        public decimal? insurancePercentage { get; init; }
        public string? interstatePurchaseReason { get; init; }
        public int? taxRuleCode { get; init; }
    }
}
