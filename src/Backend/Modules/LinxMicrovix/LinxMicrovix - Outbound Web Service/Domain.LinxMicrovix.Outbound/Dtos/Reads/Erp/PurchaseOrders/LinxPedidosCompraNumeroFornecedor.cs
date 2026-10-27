namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosCompraNumeroFornecedor: o numero do fornecedor do pedido de compra. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosCompraNumeroFornecedor
    {
        public string? portal { get; init; }
        public string? id_pedido_fornecedor { get; init; }
        public string? numero_ped_erp { get; init; }
        public string? numero_ped_fornecedor { get; init; }
        public string? timestamp { get; init; }
    }
}
