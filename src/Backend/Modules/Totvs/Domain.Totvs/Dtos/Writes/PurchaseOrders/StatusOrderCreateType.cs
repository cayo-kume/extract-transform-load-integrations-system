namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// situação do pedido de compra na inclusão (StatusOrderCreateType): vai como número (T3)
    /// </summary>
    public enum StatusOrderCreateType
    {
        InProgress = 1,   // em andamento
        Blocked = 5   // bloqueado
    }
}
