namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPedidosStatus (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPedidosStatus
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
