namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRemessasRetornoTipos: os dados da tabela remessa_retorno_tipos no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRemessasRetornoTipos
    {
        public string? portal { get; init; }
        public string? id_remessa_retorno_tipos { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
