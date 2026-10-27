namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosFlags (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosFlags
    {
        public string? portal { get; init; }
        public string? id_b2c_flags_produtos { get; init; }
        public string? id_b2c_flags { get; init; }
        public string? codigoproduto { get; init; }
        public string? timestamp { get; init; }
        public string? descricao_b2c_flags { get; init; }
    }
}
