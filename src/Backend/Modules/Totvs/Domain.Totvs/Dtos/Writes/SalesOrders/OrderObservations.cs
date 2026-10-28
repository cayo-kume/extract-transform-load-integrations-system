namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/observations-order (UpdateOrderObservationsCommand): **inclui** linhas de observação no pedido e na
    /// nota. Repetir duplica as linhas: a rota tem a trava de repetição (T5)
    /// </summary>
    public class OrderObservations : OrderKey
    {
        public List<OrderObservation>? observations { get; init; }
        public List<InvoiceObservation>? invoiceObservations { get; init; }
    }
}
