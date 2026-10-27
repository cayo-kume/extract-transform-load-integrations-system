namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosImagensURL: as URLs das imagens dos produtos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosImagensURL
    {
        public string? codigoproduto { get; init; }
        public string? url_imagem_blob { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? id_imagem { get; init; }
    }
}
