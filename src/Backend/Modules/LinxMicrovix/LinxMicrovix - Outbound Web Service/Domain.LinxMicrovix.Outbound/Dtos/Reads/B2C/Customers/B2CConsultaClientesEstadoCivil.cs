namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesEstadoCivil (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClientesEstadoCivil
    {
        public string? id_estado_civil { get; init; }
        public string? estado_civil { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
