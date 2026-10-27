namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNfeIbgeMunicipio: dados do IBGE sobre municípios. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNfeIbgeMunicipio
    {
        public string? id_nfe_ibge_municipio { get; init; }
        public string? cod_ibge_municipio { get; init; }
        public string? nome { get; init; }
        public string? cod_ibge_uf { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
