namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxAcoesPromocionaisProdutosCortesia: os produtos de cortesia de ações promocionais. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxAcoesPromocionaisProdutosCortesia
    {
        public string? id_acoes_promocionais_produtos_cortesia { get; init; }
        public string? id_acoes_promocionais { get; init; }
        public string? codigoproduto { get; init; }
        public string? id_combinacao_produto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
