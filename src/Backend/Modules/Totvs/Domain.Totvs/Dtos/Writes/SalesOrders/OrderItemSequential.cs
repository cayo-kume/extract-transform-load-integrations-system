namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// serial do item com frete e desconto (AddOrderItemsSequentialsInDto)
    /// </summary>
    public class OrderItemSequential
    {
        public string? code { get; init; }
        public decimal? freightValue { get; init; }
        public decimal? discountValue { get; init; }
    }
}
