namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNFeEvento: os dados eventos da NFe. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNFeEvento
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_nfe_evento { get; init; }
        public string? id_nfe { get; init; }
        public string? codigo_tipo { get; init; }
        public string? xml { get; init; }
        public string? timestamp { get; init; }
        public string? data_emissao { get; init; }
        public string? hora_emissao { get; init; }
    }
}
