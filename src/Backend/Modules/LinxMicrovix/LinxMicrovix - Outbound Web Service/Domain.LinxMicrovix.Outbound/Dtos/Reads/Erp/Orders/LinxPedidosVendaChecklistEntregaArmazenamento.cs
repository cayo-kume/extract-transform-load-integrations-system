namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosVendaChecklistEntregaArmazenamento: os tipos de armazenamentos cadastrados no checklist de entrega do pedido de venda. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaArmazenamento
    {
        public string? id_checklist_entrega_armazenamento { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
