namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrdemServicoExternaDav: as ordens de serviço externas das DAVs (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrdemServicoExternaDav
    {
        public string? id_vendas_pos_tmp_ordem_servico_externa { get; init; }
        public string? id_vendas_pos { get; init; }
        public string? codigo_ordem_servico_externa { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
