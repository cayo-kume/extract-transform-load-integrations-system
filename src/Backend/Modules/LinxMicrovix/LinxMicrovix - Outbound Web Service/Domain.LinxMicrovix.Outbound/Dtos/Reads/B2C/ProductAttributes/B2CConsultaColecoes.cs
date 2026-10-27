namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaColecoes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaColecoes
    {
        public string? codigo_colecao { get; init; }
        public string? nome_colecao { get; init; }
        public string? timestamp { get; init; }
        public string? marcas { get; init; }
        public string? portal { get; init; }
    }
}
