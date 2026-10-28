namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// situações aceitas pela troca de situação do pedido de venda (UpdateStatusOrderType): o swagger declara o enum como string, então vai pelo
    /// nome. Atendimento e cancelamento são de outras rotas
    /// </summary>
    public static class OrderStatusNames
    {
        public static readonly IReadOnlyList<string> Values = ["InProgress", "BillingReleased", "Blocked", "InComposition", "InAnalysis"];
    }
}
