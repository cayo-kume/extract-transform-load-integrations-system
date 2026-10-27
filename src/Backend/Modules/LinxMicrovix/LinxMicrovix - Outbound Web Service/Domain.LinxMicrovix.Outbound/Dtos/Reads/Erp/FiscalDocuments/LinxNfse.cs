namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNfse: nota Fiscal de Serviços Eletrônica (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNfse
    {
        public string? portal { get; init; }
        public string? documento { get; init; }
        public string? id_nfse { get; init; }
        public string? codigo_verificacao { get; init; }
        public string? id_nfse_situacao { get; init; }
        public string? identificador { get; init; }
        public string? dt_insert { get; init; }
        public string? timestamp { get; init; }
    }
}
