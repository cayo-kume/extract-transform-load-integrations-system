namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaVendedores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaVendedores
    {
        public string? cod_vendedor { get; init; }
        public string? nome_vendedor { get; init; }
        public string? comissao_produtos { get; init; }
        public string? comissao_servicos { get; init; }
        public string? tipo { get; init; }
        public string? ativo { get; init; }
        public string? comissionado { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
