namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// item com a quantidade nova (PurchaseOrderResquestedQuantityItemModel)
    /// </summary>
    public class RequestedQuantityItem
    {
        public int? productCode { get; init; }
        public decimal? quantity { get; init; }
    }
}
