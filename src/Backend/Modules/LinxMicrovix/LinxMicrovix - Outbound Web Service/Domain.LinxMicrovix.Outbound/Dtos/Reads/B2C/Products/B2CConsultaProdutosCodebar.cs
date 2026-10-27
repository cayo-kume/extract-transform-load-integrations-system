namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCodebar (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCodebar
    {
        public string? codigoproduto { get; init; }
        public string? codebar { get; init; }
        public string? id_produtos_codebar { get; init; }
        public string? principal { get; init; }
        public string? empresa { get; init; }
        public string? timestamp { get; init; }
        public string? tipo_codebar { get; init; }
        public string? portal { get; init; }
    }
}
