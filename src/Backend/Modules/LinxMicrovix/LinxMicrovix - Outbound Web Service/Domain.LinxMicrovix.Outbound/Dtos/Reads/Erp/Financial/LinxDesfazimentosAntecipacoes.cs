namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDesfazimentosAntecipacoes: os valores registrados na tabela desfazimentos_antecipacoes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDesfazimentosAntecipacoes
    {
        public string? portal { get; init; }
        public string? id_desfazimentos_antecipacoes { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? data_desfazimento { get; init; }
        public string? valor_desfazimento { get; init; }
        public string? valor_acerto { get; init; }
        public string? tipo_acerto { get; init; }
        public string? observacoes { get; init; }
        public string? usuario { get; init; }
        public string? timestamp { get; init; }
    }
}
