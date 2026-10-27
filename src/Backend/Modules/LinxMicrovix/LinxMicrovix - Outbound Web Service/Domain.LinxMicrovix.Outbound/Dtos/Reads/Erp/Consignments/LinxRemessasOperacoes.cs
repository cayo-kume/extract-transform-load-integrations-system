namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRemessasOperacoes: os dados da tabela remessa_operacoes no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRemessasOperacoes
    {
        public string? portal { get; init; }
        public string? id_remessa_operacoes { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
