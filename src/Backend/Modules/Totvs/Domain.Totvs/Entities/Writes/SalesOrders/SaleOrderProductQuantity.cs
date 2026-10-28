namespace Domain.Totvs.Entities.Writes.SalesOrders
{
    /// <summary>
    /// produto de um pedido de venda gravado, com a quantidade atual e a pendente: conferências locais da quantidade e do cancelamento dos itens (SO7)
    /// </summary>
    public class SaleOrderProductQuantity
    {
        public int productCode { get; set; }
        public string? productSku { get; set; }
        public decimal? quantity { get; set; }
        public decimal? pendingQuantity { get; set; }
    }
}
