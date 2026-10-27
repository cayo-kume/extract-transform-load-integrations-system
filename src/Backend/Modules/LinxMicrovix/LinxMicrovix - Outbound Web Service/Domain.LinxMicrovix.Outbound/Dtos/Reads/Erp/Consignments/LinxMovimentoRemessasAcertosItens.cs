namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoRemessasAcertosItens: os dados da tabela remessas_acertos_itens no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoRemessasAcertosItens
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_remessas_acertos { get; init; }
        public string? transacao { get; init; }
        public string? qtde_total { get; init; }
        public string? id_remessa_operacoes { get; init; }
        public string? id_remessas_itens { get; init; }
        public string? timestamp { get; init; }
    }
}
