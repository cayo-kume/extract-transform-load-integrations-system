namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaTiposCobrancaFrete (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaTiposCobrancaFrete
    {
        public string? codigo_tipo_cobranca_frete { get; init; }
        public string? nome_tipo_cobranca_frete { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
