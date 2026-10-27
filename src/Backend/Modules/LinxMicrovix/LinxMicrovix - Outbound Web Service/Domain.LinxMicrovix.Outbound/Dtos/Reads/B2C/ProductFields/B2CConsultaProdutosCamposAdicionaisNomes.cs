namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCamposAdicionaisNomes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisNomes
    {
        public string? id_campo { get; init; }
        public string? ordem { get; init; }
        public string? legenda { get; init; }
        public string? tipo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
