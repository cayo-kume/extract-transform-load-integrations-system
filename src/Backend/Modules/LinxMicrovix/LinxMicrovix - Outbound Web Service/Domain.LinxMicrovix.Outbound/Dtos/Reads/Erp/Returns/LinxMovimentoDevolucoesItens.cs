namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoDevolucoesItens: os dados da tabela movimento_devolucoes_itens cadastrados no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoDevolucoesItens
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador_venda { get; init; }
        public string? identificador_devolucao { get; init; }
        public string? codigoproduto { get; init; }
        public string? id_movimento_devolucoes_itens { get; init; }
        public string? transacao_origem { get; init; }
        public string? transacao_devolucao { get; init; }
        public string? qtde_devolvida { get; init; }
        public string? timestamp { get; init; }
    }
}
