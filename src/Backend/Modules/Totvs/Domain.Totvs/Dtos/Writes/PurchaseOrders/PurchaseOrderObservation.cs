namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// linha de observação do pedido de compra (PurchaseOrderObservationModel)
    /// </summary>
    public class PurchaseOrderObservation
    {
        public string? observation { get; init; }
        public VisualizationObservationType? visualizationType { get; init; }
    }
}
