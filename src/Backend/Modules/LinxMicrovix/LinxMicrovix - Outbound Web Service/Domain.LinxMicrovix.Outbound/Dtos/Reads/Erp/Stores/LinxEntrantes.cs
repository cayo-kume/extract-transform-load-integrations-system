namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxEntrantes: contagem de entrantes por período e loja, só dos últimos 2 anos (colunas da resposta real, 26/10/2026; a coluna entrantes não está na v274)
    /// </summary>
    public class LinxEntrantes
    {
        public string? id_entrantes { get; init; }
        public string? data_inicio { get; init; }
        public string? data_fim { get; init; }
        public string? cnpj_empresa { get; init; }
        public string? entrantes { get; init; }
        public string? portal { get; init; }
        public string? timestamp { get; init; }
    }
}
