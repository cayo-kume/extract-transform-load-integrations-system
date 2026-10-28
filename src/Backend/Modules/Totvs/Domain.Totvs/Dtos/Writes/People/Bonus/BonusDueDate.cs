namespace Domain.Totvs.Dtos.Writes.People.Bonus
{
    /// <summary>
    /// corpo do POST person/v2/bonus-due-date (BonusDueDateCommand): gera crédito de bônus desconto para o cliente, com validade. Efeito
    /// financeiro e sem estorno pela API; a origem (transação, fatura ou liquidação) é opcional, mas cada grupo vai completo
    /// </summary>
    public class BonusDueDate
    {
        public int? branchCode { get; init; }
        public int? personCode { get; init; }
        // 1 a 15, tipos da descrição do swagger (classificação do produto, valor de venda e documento...)
        public int? bonusType { get; init; }
        public DateTime? movementDate { get; init; }
        public decimal? bonusValue { get; init; }
        public decimal? usedValue { get; init; }
        public DateTime? startDate { get; init; }
        public DateTime? endDate { get; init; }
        public int? transactionBranchCode { get; init; }
        public DateTime? transactionDate { get; init; }
        public int? transactionNumber { get; init; }
        public int? invoiceBranchCode { get; init; }
        public int? invoiceClientCode { get; init; }
        public int? invoiceNumber { get; init; }
        public int? installmentNumber { get; init; }
        public int? settlementBranchCode { get; init; }
        public DateTime? settlementDate { get; init; }
        public int? settlementSequenceNumber { get; init; }
        public string? historic { get; init; }
    }
}
