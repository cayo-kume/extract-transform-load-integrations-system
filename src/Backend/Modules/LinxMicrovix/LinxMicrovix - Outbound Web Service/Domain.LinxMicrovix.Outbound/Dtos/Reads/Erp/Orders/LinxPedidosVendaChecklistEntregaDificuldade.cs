namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosVendaChecklistEntregaDificuldade: os tipos de dificuldades cadastradas no checklist de entrega do pedido de venda. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaDificuldade
    {
        public string? id_checklist_entrega_dificuldade { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
