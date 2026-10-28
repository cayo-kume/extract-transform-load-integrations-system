namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// despesa da parcela (ExpenseModel): o rateio vai por percentual ou por valor, um ou outro (a descrição do swagger manda isso,
    /// embora o schema marque o percentual como obrigatório)
    /// </summary>
    public class Expense
    {
        public int? expenseCode { get; init; }
        public int? costCenterCode { get; init; }
        public decimal? proratedPercentage { get; init; }
        public decimal? proratedValue { get; init; }
    }
}
