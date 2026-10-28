namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Observação manual. (RepresentativeObservationInDto)
    /// </summary>
    public class B2COrderRepresentativeObservation
    {
        // Linha de observação.
        public string? observation { get; init; }
        public VisualizationObservationType? visualizationType { get; init; }
    }
}
