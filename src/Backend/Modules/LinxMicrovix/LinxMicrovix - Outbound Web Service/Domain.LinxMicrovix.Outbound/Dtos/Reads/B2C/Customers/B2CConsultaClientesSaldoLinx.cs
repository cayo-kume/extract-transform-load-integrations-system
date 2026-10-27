namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesSaldoLinx (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClientesSaldoLinx
    {
        public string? cod_cliente_erp { get; init; }
        public string? cod_cliente_b2c { get; init; }
        public string? empresa { get; init; }
        public string? valor { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
