namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDevolucaoRemanejoFabricaStatus: o cadastro de status da rotina devoluções e remanejos de fábrica. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaStatus
    {
        public string? id_devolucao_remanejo_fabrica_status { get; init; }
        public string? id_devolucao_remanejo_fabrica_tipo { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
