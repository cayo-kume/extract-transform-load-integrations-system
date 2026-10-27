namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoTrocas: os valores e números das trocas realizadas nas vendas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoTrocas
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? num_vale { get; init; }
        public string? valor_vale { get; init; }
        public string? motivo { get; init; }
        public string? doc_origem { get; init; }
        public string? serie_origem { get; init; }
        public string? doc_venda { get; init; }
        public string? serie_venda { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
        public string? desfazimento { get; init; }
        public string? empresa { get; init; }
        public string? vale_cod_cliente { get; init; }
        public string? vale_codigoproduto { get; init; }
        public string? id_vale_ordem_servico_externa { get; init; }
        public string? doc_venda_origem { get; init; }
        public string? serie_venda_origem { get; init; }
        public string? cod_cliente { get; init; }
        public string? data_vencimento { get; init; }
        public string? valor_original { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? obs_troca { get; init; }
    }
}
