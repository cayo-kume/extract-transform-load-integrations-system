namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosCodBar: o cadastro de códigos de barras dos produtos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosCodBar
    {
        public string? portal { get; init; }
        public string? cod_produto { get; init; }
        public string? cod_barra { get; init; }
        public string? timestamp { get; init; }
    }
}
