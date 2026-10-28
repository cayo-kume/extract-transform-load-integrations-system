namespace Domain.Totvs.Entities.Writes.PurchaseOrders
{
    /// <summary>
    /// produto de um pedido de compra gravado, com a quantidade já recebida: conferência local da troca de quantidade (§6.2)
    /// </summary>
    public class OrderProductQuantity
    {
        public int productCode { get; set; }
        public decimal? settledQuantity { get; set; }
    }
}
