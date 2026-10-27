namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxBalancosSecoes: as seções dos balanços. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxBalancosSecoes
    {
        public string? portal { get; init; }
        public string? id_balancos_secoes { get; init; }
        public string? id_balancos { get; init; }
        public string? id_balancos_arquivos { get; init; }
        public string? status { get; init; }
        public string? timestamp { get; init; }
    }
}
