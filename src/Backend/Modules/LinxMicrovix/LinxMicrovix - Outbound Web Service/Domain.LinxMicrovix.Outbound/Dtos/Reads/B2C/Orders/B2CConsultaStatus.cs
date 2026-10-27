namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaStatus (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaStatus
    {
        public string? id_status { get; init; }
        public string? descricao_status { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
