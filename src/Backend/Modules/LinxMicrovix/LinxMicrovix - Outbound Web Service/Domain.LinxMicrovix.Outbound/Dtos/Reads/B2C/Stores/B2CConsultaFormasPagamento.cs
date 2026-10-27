namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaFormasPagamento (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaFormasPagamento
    {
        public string? cod_forma_pgto { get; init; }
        public string? forma_pgto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
