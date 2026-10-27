namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNfceEstacao: cadastro de estações. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNfceEstacao
    {
        public string? id_nfce_estacao { get; init; }
        public string? empresa { get; init; }
        public string? descricao { get; init; }
        public string? numero_pdv_tef { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
