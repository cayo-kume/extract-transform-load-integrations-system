namespace Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// lote de pedidos de venda do PEDFM018 (GET sales-order/v2/batch-items, BatchOrderModel; o código vai em batchOrder da capa do pedido; status 1 aberto, 2 fechado)
    /// </summary>
    public class SaleOrderBatch
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? status { get; init; }
        public string? startDate { get; init; }
        public string? endDate { get; init; }
        public string? closingDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
