namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesSaldo (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClientesSaldo
    {
        public string? saldo { get; init; }
        public string? cod_cliente_erp { get; init; }
        public string? empresa { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
