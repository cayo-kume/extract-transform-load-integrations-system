namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosDimensoes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosDimensoes
    {
        public string? codigoproduto { get; init; }
        public string? altura { get; init; }
        public string? comprimento { get; init; }
        public string? timestamp { get; init; }
        public string? largura { get; init; }
        public string? portal { get; init; }
    }
}
