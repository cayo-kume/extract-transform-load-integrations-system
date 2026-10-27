namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoOrdemServicoExterna: a ordem de serviço externa da movimentação. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoOrdemServicoExterna
    {
        public string? id_movimento_ordem_servico_externa { get; init; }
        public string? identificador { get; init; }
        public string? codigo_ordem_servico { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
