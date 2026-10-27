namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoRemessas: os dados da tabela remessas no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoRemessas
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_remessas { get; init; }
        public string? id_tipo { get; init; }
        public string? identificador_remessa { get; init; }
        public string? status { get; init; }
        public string? status_descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
