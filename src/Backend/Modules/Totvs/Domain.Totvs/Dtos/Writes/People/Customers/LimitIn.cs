namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// limite de crédito por loja (LimitInDto): efeito financeiro
    /// </summary>
    public class LimitIn
    {
        public int? branchCode { get; init; }
        public decimal? limitValue { get; init; }
        public decimal? saleLimitValue { get; init; }
        public decimal? monthlyLimitValue { get; init; }
    }
}
