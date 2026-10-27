namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoObservacoes: o conteúdo da tabela movimento_observacoes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoObservacoes
    {
        public string? portal { get; init; }
        public string? id_obs { get; init; }
        public string? titulo_obs { get; init; }
        public string? descricao_obs { get; init; }
        public string? timestamp { get; init; }
    }
}
