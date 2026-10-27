namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxExtratoBancarioConciliacoes: os registros da tabela extrato_bancario_conciliacoes. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxExtratoBancarioConciliacoes
    {
        public string? portal { get; init; }
        public string? id_extrato_bancario_conciliacoes { get; init; }
        public string? id_extrato_bancario_lancamentos { get; init; }
        public string? codigo_fatura { get; init; }
        public string? dados_anteriores_baixa_fatura { get; init; }
        public string? timestamp { get; init; }
    }
}
