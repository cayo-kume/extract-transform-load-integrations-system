namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClassificacao (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClassificacao
    {
        public string? codigo_classificacao { get; init; }
        public string? nome_classificacao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
