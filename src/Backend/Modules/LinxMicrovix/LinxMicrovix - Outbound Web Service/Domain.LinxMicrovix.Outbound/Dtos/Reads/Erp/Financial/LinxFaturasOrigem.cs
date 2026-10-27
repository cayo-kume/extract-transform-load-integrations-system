namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFaturasOrigem: as faturas de origem (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFaturasOrigem
    {
        public string? codigo_fatura { get; init; }
        public string? codigo_fatura_origem { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
