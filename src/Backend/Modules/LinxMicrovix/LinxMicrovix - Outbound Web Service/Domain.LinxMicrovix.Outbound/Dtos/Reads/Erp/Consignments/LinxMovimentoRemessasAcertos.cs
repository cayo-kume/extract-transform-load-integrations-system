namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoRemessasAcertos: os dados da tabela remessas_acertos no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoRemessasAcertos
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_remessas_acertos { get; init; }
        public string? id_remessas { get; init; }
        public string? identificador_venda { get; init; }
        public string? identificador_retorno { get; init; }
        public string? identificador_devolucao { get; init; }
        public string? data { get; init; }
        public string? id_vendas_pos { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
    }
}
