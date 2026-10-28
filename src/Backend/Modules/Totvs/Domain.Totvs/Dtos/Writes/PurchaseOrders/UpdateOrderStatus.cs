namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// corpo do POST purchase-order/v2/orders/change-status (UpdateOrderStatusCommand): põe um pedido de compra em andamento ou bloqueado.
    /// Recebido pela rota do controller e enviado à Totvs como está; nulos não vão
    /// </summary>
    public class UpdateOrderStatus
    {
        // loja do pedido; precisa ser da empresa em totvs.TotvsBranches
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }

        public StatusOrderChange? status { get; init; }
    }
}
