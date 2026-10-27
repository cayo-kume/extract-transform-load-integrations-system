namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCamposAdicionaisDetalhes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisDetalhes
    {
        public string? id_campo_detalhe { get; init; }
        public string? ordem { get; init; }
        public string? descricao { get; init; }
        public string? id_campo { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
