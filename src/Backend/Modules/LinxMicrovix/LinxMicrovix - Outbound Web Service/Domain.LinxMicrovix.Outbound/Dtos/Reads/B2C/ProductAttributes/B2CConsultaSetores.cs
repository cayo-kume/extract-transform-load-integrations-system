namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaSetores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaSetores
    {
        public string? codigo_setor { get; init; }
        public string? nome_setor { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
