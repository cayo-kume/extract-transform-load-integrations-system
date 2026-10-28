namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// uma linha de observação da nota (InvoiceObservationCommandModel)
    /// </summary>
    public class InvoiceObservation
    {
        public string? observation { get; init; }   // até 80
    }
}
