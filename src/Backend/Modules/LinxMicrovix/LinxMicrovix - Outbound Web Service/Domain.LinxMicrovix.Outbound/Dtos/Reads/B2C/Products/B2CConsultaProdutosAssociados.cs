namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosAssociados (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosAssociados
    {
        public string? id { get; init; }
        public string? codigoproduto { get; init; }
        public string? codigoproduto_associado { get; init; }
        public string? coeficiente_desconto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? qtde_item { get; init; }
        public string? item_obrigatorio { get; init; }
    }
}
