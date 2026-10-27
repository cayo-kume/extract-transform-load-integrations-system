namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesContatosParentesco (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClientesContatosParentesco
    {
        public string? id_parentesco { get; init; }
        public string? descricao_parentesco { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
