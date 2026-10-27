namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosInformacoes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosInformacoes
    {
        public string? id_produtos_informacoes { get; init; }
        public string? codigoproduto { get; init; }
        public string? informacoes_produto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
