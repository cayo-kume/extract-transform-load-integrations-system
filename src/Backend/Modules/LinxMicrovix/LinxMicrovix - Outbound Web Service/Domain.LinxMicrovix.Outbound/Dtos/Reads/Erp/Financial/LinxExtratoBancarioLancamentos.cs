namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxExtratoBancarioLancamentos: os registros da tabela extrato_bancario_lancamentos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxExtratoBancarioLancamentos
    {
        public string? portal { get; init; }
        public string? id_extrato_bancario_lancamentos { get; init; }
        public string? codigo_banco { get; init; }
        public string? numero_conta { get; init; }
        public string? id_conta { get; init; }
        public string? valor { get; init; }
        public string? descricao { get; init; }
        public string? data_lancamento { get; init; }
        public string? timestamp { get; init; }
    }
}
