namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Desconto. (TypeDiscountInDto)
    /// </summary>
    public class B2COrderDiscount
    {
        // Código do tipo de desconto.
        public int? typeDiscountCode { get; init; }
        // Valor de desconto.
        public decimal? discountValue { get; init; }
        // Percentual de desconto.
        public decimal? discountPercentage { get; init; }
    }
}
