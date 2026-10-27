namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosImagensURL (colunas da V64: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class B2CConsultaProdutosImagensURL
    {
        public string? codigoproduto { get; init; }
        public string? url_imagem_blob { get; init; }
        public string? tipo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
