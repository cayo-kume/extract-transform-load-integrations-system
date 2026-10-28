namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// item de lote ligado (RelationshipBatchItemModel): por loja + lote + sequência ou pela barra (o schema marca os quatro)
    /// </summary>
    public class OrderBatchItem
    {
        public int? branchCode { get; init; }
        public int? code { get; init; }
        public int? sequence { get; init; }
        public string? barCode { get; init; }
        public decimal? quantity { get; init; }
    }
}
