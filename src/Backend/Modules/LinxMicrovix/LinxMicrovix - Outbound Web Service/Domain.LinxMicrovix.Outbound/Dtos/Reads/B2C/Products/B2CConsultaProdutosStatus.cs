namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosStatus (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosStatus
    {
        public string? codigoproduto { get; init; }
        public string? referencia { get; init; }
        public string? ativo { get; init; }
        public string? b2c { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
