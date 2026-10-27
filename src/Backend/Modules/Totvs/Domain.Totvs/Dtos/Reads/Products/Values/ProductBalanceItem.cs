namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// saldo de um produto numa loja (ProductBalanceItemModel)
    /// </summary>
    public class ProductBalanceItem
    {
        public string? branchCode { get; init; }
        public string? stockCode { get; init; }
        public string? stockDescription { get; init; }
        public string? stock { get; init; }
        public string? salesOrder { get; init; }
        public string? inputTransaction { get; init; }
        public string? outputTransaction { get; init; }
        public string? productionPlanning { get; init; }
        public string? purchaseOrder { get; init; }
        public string? productionOrderProgress { get; init; }
        public string? productionOrderWaitLib { get; init; }
        public string? stockTemp { get; init; }
    }
}
