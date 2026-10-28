namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// situações aceitas pela troca de situação do pedido de compra (StatusOrderChange), com os números da descrição do swagger: vai como
    /// número (D6/T3). Os números são os do StatusOrderType, que a busca devolve e que fica em totvs.TotvsPurchaseOrders.status
    /// </summary>
    public enum StatusOrderChange
    {
        InProgress = 1,   // em andamento
        Blocked = 5       // bloqueado
    }
}
