namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxHistoricosContabeis: os dados dos históricos contábeis. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxHistoricosContabeis
    {
        public string? id_historico { get; init; }
        public string? nome_historico { get; init; }
        public string? conta_credito { get; init; }
        public string? conta_debito { get; init; }
        public string? tipo_rateio { get; init; }
        public string? inativo { get; init; }
        public string? operacao { get; init; }
        public string? historico_contabil { get; init; }
        public string? somente_gerencial { get; init; }
        public string? obrigatorio_insercao_anexo_sangria { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
