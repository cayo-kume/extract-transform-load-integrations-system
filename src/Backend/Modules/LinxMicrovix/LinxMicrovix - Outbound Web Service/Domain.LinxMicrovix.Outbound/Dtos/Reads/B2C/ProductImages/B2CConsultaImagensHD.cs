namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaImagensHD (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaImagensHD
    {
        public string? portal { get; init; }
        public string? identificador_imagem { get; init; }
        public string? codigoproduto { get; init; }
        public string? imagem { get; init; }
        public string? timestamp { get; init; }
        public string? url_imagem_blob { get; init; }
    }
}
