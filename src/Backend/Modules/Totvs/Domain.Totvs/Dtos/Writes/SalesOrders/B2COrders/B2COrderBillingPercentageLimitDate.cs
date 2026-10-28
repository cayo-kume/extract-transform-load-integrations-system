namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Limitador de percentual sugerido por data de faturamento. (BillingPercentageLimitDateInDto)
    /// </summary>
    public class B2COrderBillingPercentageLimitDate
    {
        // Data limite de faturamento.
        public DateTime? billingForecastDate { get; init; }
        // Percentual de limite de faturamento.
        public decimal? billingPercentageLimit { get; init; }
        public B2COrderLimitType? limitType { get; init; }
    }
}
