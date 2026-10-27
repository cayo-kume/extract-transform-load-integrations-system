namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMotivosDesoneracaoIcms: o conteúdo da tabela motivos_desoneracao_icms (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMotivosDesoneracaoIcms
    {
        public string? portal { get; init; }
        public string? id_motivos_desoneracao_icms { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
