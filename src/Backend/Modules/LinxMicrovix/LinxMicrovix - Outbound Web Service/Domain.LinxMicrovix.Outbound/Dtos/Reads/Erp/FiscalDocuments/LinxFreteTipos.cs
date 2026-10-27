namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFreteTipos: tipos de frete. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFreteTipos
    {
        public string? id_frete_tipos { get; init; }
        public string? descricao { get; init; }
        public string? codigo_sped { get; init; }
        public string? codigo_nfe { get; init; }
        public string? codigo_sintegra { get; init; }
        public string? permite_valor { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
