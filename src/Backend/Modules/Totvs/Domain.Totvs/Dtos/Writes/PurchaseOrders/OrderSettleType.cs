namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// tipo de baixa do pedido de compra: vai como número (T3). Anulável no DTO: o 0 mandaria "total e parcial" sem ninguém pedir
    /// </summary>
    public enum OrderSettleType
    {
        TotalAndPartial = 0,   // total e parcial
        OnlyTotal = 1   // somente total
    }
}
