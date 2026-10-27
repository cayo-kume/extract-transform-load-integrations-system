namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxTrocaUnificadaResumoVendasItens (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxTrocaUnificadaResumoVendasItens
    {
        public string? id_troca_unificada_resumo_vendas_itens { get; init; }
        public string? id_troca_unificada_resumo_vendas { get; init; }
        public string? codigoproduto { get; init; }
        public string? transacao { get; init; }
        public string? serial { get; init; }
        public string? valor_liquido { get; init; }
        public string? data_validade { get; init; }
        public string? venda_referenciada { get; init; }
        public string? token_utilizado { get; init; }
        public string? quantidade { get; init; }
        public string? timestamp { get; init; }
    }
}
