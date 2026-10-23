namespace Domain.Totvs.Dtos.Reads.AccountsPayable
{
    /// <summary>
    /// despesa (rateio) da duplicata
    /// </summary>
    public class Expense
    {
        public string? expenseCode { get; init; }
        public string? expenseName { get; init; }
        public string? costCenterCode { get; init; }
        public string? proratedPercentage { get; init; }
        public string? proratedValue { get; init; }
    }
}
