namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxB2CPedidosStatus: o status dos pedidos de B2C (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxB2CPedidosStatus
    {
        public string? id { get; init; }
        public string? id_status { get; init; }
        public string? id_pedido { get; init; }
        public string? data_hora { get; init; }
        public string? anotacao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
