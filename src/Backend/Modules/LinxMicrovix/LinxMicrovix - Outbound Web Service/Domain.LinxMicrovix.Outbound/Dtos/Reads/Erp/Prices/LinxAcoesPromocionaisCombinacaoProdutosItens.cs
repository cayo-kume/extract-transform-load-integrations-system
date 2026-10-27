namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxAcoesPromocionaisCombinacaoProdutosItens: os itens da combinação de produtos de ações promocionais. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxAcoesPromocionaisCombinacaoProdutosItens
    {
        public string? id_acoes_promocionais_combinacao_produtos_itens { get; init; }
        public string? id_combinacao_produto { get; init; }
        public string? codigoproduto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
