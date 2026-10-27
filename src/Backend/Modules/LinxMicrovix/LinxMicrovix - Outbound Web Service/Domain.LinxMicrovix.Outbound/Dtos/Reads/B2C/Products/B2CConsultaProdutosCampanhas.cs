namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCampanhas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCampanhas
    {
        public string? codigo_campanha { get; init; }
        public string? nome_campanha { get; init; }
        public string? vigencia_inicio { get; init; }
        public string? vigencia_fim { get; init; }
        public string? observacao { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
