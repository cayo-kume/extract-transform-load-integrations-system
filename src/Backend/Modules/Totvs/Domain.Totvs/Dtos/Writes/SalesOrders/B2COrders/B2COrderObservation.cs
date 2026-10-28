namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Observação manual. (ObservationInDto)
    /// </summary>
    public class B2COrderObservation
    {
        // Linha de observação.
        public string? observation { get; init; }
        public VisualizationObservationType? visualizationType { get; init; }
    }
}
