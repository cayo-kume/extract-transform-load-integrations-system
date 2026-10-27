namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxEnquadramentoIpi: o conteúdo da tabela enquadramento_ipi (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxEnquadramentoIpi
    {
        public string? portal { get; init; }
        public string? id_enquadramento_ipi { get; init; }
        public string? codigo { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
