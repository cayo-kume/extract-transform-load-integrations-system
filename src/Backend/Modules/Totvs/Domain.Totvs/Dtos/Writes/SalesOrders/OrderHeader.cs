namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/header (UpdateOrderCommand): altera a capa do pedido. averagePaymentPeriod não é anulável no swagger:
    /// aqui é, e fica de fora quando nulo (SO8)
    /// </summary>
    public class OrderHeader : OrderKey
    {
        public int? averagePaymentPeriod { get; init; }
        public int? operationCode { get; init; }
        public int? priceTableCode { get; init; }
        public int? priorityCode { get; init; }
    }
}
