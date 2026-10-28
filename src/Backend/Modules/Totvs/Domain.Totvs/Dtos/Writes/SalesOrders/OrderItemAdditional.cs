namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// um item do OrderItemsAdditional (OrderItemUpdateModel): o produto pelo código ou pelo SKU, um ou outro
    /// </summary>
    public class OrderItemAdditional
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }      // até 40

        public int? rollMeasureCode { get; init; }    // até 9 dígitos
        public DateTime? billingForecastDate { get; init; }
    }
}
