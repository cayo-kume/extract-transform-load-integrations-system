namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCamposAdicionaisValores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisValores
    {
        public string? id_campo_valor { get; init; }
        public string? codigoproduto { get; init; }
        public string? id_campo_detalhe { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
