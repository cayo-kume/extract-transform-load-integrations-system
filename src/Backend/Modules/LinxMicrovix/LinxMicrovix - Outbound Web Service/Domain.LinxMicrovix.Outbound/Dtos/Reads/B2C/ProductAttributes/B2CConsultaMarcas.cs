namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaMarcas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaMarcas
    {
        public string? codigo_marca { get; init; }
        public string? nome_marca { get; init; }
        public string? timestamp { get; init; }
        public string? linhas { get; init; }
        public string? portal { get; init; }
    }
}
