namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// dados adicionais de tributo da duplicata (TaxAdditionalModel)
    /// </summary>
    public class TaxAdditional
    {
        public int? revenue { get; init; }
        public DateTime? calculationPeriod { get; init; }
        public DateTime? competenceDate { get; init; }
        public string? reference { get; init; }
        public decimal? grossRevenueValue { get; init; }
        public bool? isThirteenthSalary { get; init; }
        public decimal? otherEntitiesValue { get; init; }
    }
}
