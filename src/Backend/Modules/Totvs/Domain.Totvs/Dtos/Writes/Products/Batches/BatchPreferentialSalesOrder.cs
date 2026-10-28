namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// corpo do POST product/v2/batch-preferentialsalesorder (BatchItemPreferentialSalesOrderCommand): troca os pedidos
    /// preferenciais de um item de lote. A Totvs apaga antes todos os do item: uma lista vazia só apaga (o controller exige ?clear=true)
    /// </summary>
    public class BatchPreferentialSalesOrder
    {
        public int? branchCode { get; init; }
        public int? batchCode { get; init; }
        public int? sequence { get; init; }
        public string? batchItemBarCode { get; init; }
        public List<PreferentialSalesOrder>? preferentialSalesOrder { get; init; }
    }
}
