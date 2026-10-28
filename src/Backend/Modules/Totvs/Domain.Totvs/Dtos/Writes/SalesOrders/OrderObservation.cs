namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// uma linha de observação do pedido (ObservationCommandModel)
    /// </summary>
    public class OrderObservation
    {
        public string? observation { get; init; }   // até 80
        public VisualizationObservationType? visualizationType { get; init; }
    }
}
