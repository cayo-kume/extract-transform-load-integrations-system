namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCest: o conteúdo da tabela classificacao_cest_produto (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCest
    {
        public string? portal { get; init; }
        public string? id_cest { get; init; }
        public string? descricao { get; init; }
        public string? cest { get; init; }
        public string? id_segmento_mercadoria_bem { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
    }
}
