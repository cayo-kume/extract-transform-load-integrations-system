namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaCodigoRastreio (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaCodigoRastreio
    {
        public string? id_pedido { get; init; }
        public string? documento { get; init; }
        public string? serie { get; init; }
        public string? codigo_rastreio { get; init; }
        public string? sequencia_volume { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
