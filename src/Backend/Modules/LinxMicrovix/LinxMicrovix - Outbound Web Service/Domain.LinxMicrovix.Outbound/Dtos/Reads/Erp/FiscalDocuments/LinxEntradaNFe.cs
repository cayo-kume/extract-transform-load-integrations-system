namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxEntradaNFe: dados de NFes de entrada. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxEntradaNFe
    {
        public string? id_entrada_nfe { get; init; }
        public string? numero_nfe { get; init; }
        public string? lancada_no_microvix { get; init; }
        public string? empresa { get; init; }
        public string? documento_emitente { get; init; }
        public string? razao_social_emitente { get; init; }
        public string? serie_nfe { get; init; }
        public string? data_emissao_nfe { get; init; }
        public string? chave_nfe { get; init; }
        public string? excluida { get; init; }
        public string? dt_insert { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
