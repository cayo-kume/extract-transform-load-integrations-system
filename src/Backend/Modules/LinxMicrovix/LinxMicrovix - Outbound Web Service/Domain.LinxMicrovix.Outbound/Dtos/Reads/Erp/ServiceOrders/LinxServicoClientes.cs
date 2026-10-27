namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxServicoClientes: os dados dos clientes do serviço. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxServicoClientes
    {
        public string? portal { get; init; }
        public string? id_cliente { get; init; }
        public string? empresa { get; init; }
        public string? id_servico { get; init; }
        public string? qtd_adquirido { get; init; }
        public string? qtd_agendado { get; init; }
        public string? qtd_utilizado { get; init; }
        public string? qtd_cancelado { get; init; }
        public string? dt_insercao { get; init; }
        public string? tipo_desconto { get; init; }
        public string? valor_desconto_por_item { get; init; }
        public string? id_dav_item { get; init; }
        public string? transacao_movimento { get; init; }
        public string? ficha { get; init; }
        public string? timestamp { get; init; }
        public string? identificador { get; init; }
    }
}
