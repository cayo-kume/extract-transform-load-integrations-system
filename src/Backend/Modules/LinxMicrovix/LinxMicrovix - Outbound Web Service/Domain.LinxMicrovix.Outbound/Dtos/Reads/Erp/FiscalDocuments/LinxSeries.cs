namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSeries: as séries cadastradas no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSeries
    {
        public string? portal { get; init; }
        public string? serie { get; init; }
        public string? id_modelo_nf { get; init; }
        public string? timestamp { get; init; }
    }
}
