namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// corpo do POST purchase-order/v2/quantity-items (UpdateItemResquestedQuantityCommand): troca a quantidade pedida dos itens
    /// </summary>
    public class RequestedQuantity
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        public List<RequestedQuantityItem>? items { get; init; }
    }
}
