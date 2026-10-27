namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxVendedoresFerias: as férias cadatradas para os vendedores. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxVendedoresFerias
    {
        public string? portal { get; init; }
        public string? cod_vendedor { get; init; }
        public string? data_inicial { get; init; }
        public string? data_final { get; init; }
        public string? timestamp { get; init; }
    }
}
