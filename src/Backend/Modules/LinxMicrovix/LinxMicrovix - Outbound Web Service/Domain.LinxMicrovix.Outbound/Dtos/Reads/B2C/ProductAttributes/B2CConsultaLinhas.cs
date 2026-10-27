namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaLinhas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaLinhas
    {
        public string? codigo_linha { get; init; }
        public string? nome_linha { get; init; }
        public string? timestamp { get; init; }
        public string? setores { get; init; }
        public string? portal { get; init; }
    }
}
