namespace Domain.Totvs.Dtos.AccountsPayable
{
    public class Expense
    {
        public string expenseCode { get; set; }
        public string expenseName { get; set; }
        public string costCenterCode { get; set; }
        public string proratedPercentage { get; set; }
        public string proratedValue { get; set; }
    }
}
