namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxB2CStatus: o cadastro dos status de B2C (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxB2CStatus
    {
        public string? id_status { get; init; }
        public string? descricao_status { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
