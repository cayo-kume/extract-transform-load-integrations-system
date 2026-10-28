namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// desconto do pedido de compra (PurchaseOrderDiscountModel)
    /// </summary>
    public class PurchaseOrderDiscount
    {
        public int? discountTypeCode { get; init; }
        public decimal? discountPercentage { get; init; }
    }
}
