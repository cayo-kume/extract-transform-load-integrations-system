namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Situação do pedido. (StatusOrderInputType): vai como número (T3)
    /// </summary>
    public enum B2COrderStatus
    {
        InProgress = 1,   // InProgress(Em andamento)
        Blocked = 5,   // Blocked(Bloqueado)
        InAnalysis = 8   // InAnalysis(Em análise)
    }
}
