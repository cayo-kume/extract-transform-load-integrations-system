namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosInformacoes: informações dos produtos (características). (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosInformacoes
    {
        public string? codigoproduto { get; init; }
        public string? informacoes_produto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
