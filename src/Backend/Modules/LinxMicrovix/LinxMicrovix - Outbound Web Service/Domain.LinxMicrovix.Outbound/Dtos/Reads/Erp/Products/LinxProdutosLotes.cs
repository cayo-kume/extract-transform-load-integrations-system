namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosLotes: o saldo do lotes de produtos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosLotes
    {
        public string? portal { get; init; }
        public string? codigoproduto { get; init; }
        public string? empresa { get; init; }
        public string? lote { get; init; }
        public string? deposito { get; init; }
        public string? saldo_disponivel { get; init; }
        public string? timestamp { get; init; }
    }
}
