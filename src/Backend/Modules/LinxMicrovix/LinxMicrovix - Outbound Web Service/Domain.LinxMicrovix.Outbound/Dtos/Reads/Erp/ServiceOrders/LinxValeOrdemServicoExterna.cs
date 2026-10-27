namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxValeOrdemServicoExterna: os dados de vale ordem de serviço externa. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxValeOrdemServicoExterna
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_vale_ordem_servico_externa { get; init; }
        public string? numero_controle { get; init; }
        public string? timestamp { get; init; }
    }
}
