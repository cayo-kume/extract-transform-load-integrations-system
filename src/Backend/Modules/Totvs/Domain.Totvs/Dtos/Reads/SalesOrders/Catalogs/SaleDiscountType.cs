namespace Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// tipo de desconto do pedido de venda (GET sales-order/v2/discount-type, DiscountTypeModel)
    /// </summary>
    public class SaleDiscountType
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? percentage { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
