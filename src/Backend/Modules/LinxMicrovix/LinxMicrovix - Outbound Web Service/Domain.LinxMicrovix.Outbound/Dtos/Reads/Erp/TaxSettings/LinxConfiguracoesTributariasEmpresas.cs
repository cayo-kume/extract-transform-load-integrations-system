namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxConfiguracoesTributariasEmpresas: o conteúdo da tabela config_tributaria_empresa. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxConfiguracoesTributariasEmpresas
    {
        public string? portal { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? empresa { get; init; }
        public string? timestamp { get; init; }
    }
}
