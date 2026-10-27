namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRamosAtividade: os ramos de atividades cadastrados. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRamosAtividade
    {
        public string? portal { get; init; }
        public string? id_ramo_atividade { get; init; }
        public string? nome_ramo_atividade { get; init; }
        public string? codigo_ramo_atividade { get; init; }
        public string? timestamp { get; init; }
    }
}
