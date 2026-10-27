namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPedidosTipos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPedidosTipos
    {
        public string? id_tipo_b2c { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
