namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosAssociados: a associação entre produtos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosAssociados
    {
        public string? portal { get; init; }
        public string? codigoproduto_origem { get; init; }
        public string? codigoproduto_associado { get; init; }
        public string? coeficiente_desconto { get; init; }
        public string? timestamp { get; init; }
        public string? qtde_item { get; init; }
        public string? item_obrigatorio { get; init; }
    }
}
