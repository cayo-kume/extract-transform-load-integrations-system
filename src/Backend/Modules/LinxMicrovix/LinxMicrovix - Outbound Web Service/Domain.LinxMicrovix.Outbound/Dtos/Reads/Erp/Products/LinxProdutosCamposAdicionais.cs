namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosCamposAdicionais: os campos adicionais preenchidos dos Produtos cadastrados no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosCamposAdicionais
    {
        public string? portal { get; init; }
        public string? cod_produto { get; init; }
        public string? campo { get; init; }
        public string? valor { get; init; }
        public string? timestamp { get; init; }
        public string? id_campo_valor { get; init; }
    }
}
