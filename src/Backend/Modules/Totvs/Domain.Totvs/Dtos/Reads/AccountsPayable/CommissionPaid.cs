namespace Domain.Totvs.Dtos.Reads.AccountsPayable
{
    /// <summary>
    /// fechamento de comissão por comissionado do POST accounts-payable/v2/comissions-paid/search
    /// </summary>
    public class CommissionPaid
    {
        public string? closingBranchCode { get; init; }
        public string? closingDate { get; init; }
        public string? closingCode { get; init; }
        public string? commissionedCpfCnpj { get; init; }
        public string? commissionedName { get; init; }
        public string? commissionStatus { get; init; }
        public string? valueBilling { get; init; }
        public string? valueBillingCommission { get; init; }
        public string? valueReceiving { get; init; }
        public string? valueReceivingCommission { get; init; }
        public string? irrfValue { get; init; }
        public string? issqnValue { get; init; }
        public string? valueRebate { get; init; }
        public string? valueRebateBilling { get; init; }
        public string? valueRebateReceiving { get; init; }
        public string? discountValue { get; init; }
        public string? netValue { get; init; }
        public string? accountBalanceValue { get; init; }
        public string? grossValue { get; init; }
        public string? returnValue { get; init; }

        public List<AccountMovement>? accountMovements { get; init; }
    }
}
