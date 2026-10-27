namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosTags (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosTags
    {
        public string? portal { get; init; }
        public string? id_b2c_tags_produtos { get; init; }
        public string? id_b2c_tags { get; init; }
        public string? codigoproduto { get; init; }
        public string? timestamp { get; init; }
        public string? descricao_b2c_tags { get; init; }
    }
}
