namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosCompraStatus: os status de pedidos de compra. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosCompraStatus
    {
        public string? id_compras_pedido_status { get; init; }
        public string? descricao { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
