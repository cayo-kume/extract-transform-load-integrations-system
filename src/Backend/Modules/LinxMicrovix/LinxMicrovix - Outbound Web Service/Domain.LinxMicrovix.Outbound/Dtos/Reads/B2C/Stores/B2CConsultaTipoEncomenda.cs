namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaTipoEncomenda (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaTipoEncomenda
    {
        public string? id_tipo_encomenda { get; init; }
        public string? nm_tipo_encomenda { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
