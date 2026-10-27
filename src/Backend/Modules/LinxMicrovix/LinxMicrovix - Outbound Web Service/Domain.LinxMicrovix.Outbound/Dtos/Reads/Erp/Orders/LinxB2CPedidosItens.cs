namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxB2CPedidosItens: os itens dos pedidos de B2C (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxB2CPedidosItens
    {
        public string? id_pedido_item { get; init; }
        public string? id_pedido { get; init; }
        public string? codigoproduto { get; init; }
        public string? quantidade { get; init; }
        public string? vl_unitario { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
