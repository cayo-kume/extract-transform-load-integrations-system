namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosImagens (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosImagens
    {
        public string? id_imagem_produto { get; init; }
        public string? id_imagem { get; init; }
        public string? codigoproduto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? principal { get; init; }
    }
}
