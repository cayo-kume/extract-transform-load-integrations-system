namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// rateio de despesa do pedido ou do item (PurchaseOrderExpenseModel e PurchaseOrderItemExpenseModel)
    /// </summary>
    public class PurchaseOrderExpense
    {
        public int? expenseCode { get; init; }
        public decimal? expensePercentage { get; init; }
        public int? costCenter { get; init; }
    }
}
