namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaTags (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaTags
    {
        public string? portal { get; init; }
        public string? id_b2c_tags { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
