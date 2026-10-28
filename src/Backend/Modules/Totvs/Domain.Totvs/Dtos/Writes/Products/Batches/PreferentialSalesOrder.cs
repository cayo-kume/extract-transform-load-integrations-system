namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// pedido de venda preferencial (PreferentialSalesOrderModel): loja e pedido ou orderId
    /// </summary>
    public class PreferentialSalesOrder
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        public string? orderId { get; init; }
    }
}
