namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// parcela da duplicata (InstallmentModel). Os campos que o swagger declara sem nullable são anuláveis aqui e obrigatórios no
    /// validador: omitidos, iriam como 0 ou 0001-01-01
    /// </summary>
    public class Installment
    {
        public int? installmentCode { get; init; }
        public int? bearerCode { get; init; }
        public DateTime? issueDate { get; init; }
        public DateTime? dueDate { get; init; }
        // com prevision Real, é a data da liquidação de inclusão
        public DateTime? arrivalDate { get; init; }
        public DocumentType? document { get; init; }
        public PrevisionType? prevision { get; init; }
        public StageType? stage { get; init; }
        public decimal? duplicateValue { get; init; }

        // descontos, multa, mora e acréscimos: valores com 12 inteiros e 2 decimais, percentuais com 3 inteiros e 4 decimais
        public decimal? punctualityDiscountValue { get; init; }
        public decimal? punctualityDiscountPercentage { get; init; }
        public decimal? advanceDiscountValue1 { get; init; }
        public decimal? advanceDiscountPercentage1 { get; init; }
        public decimal? advanceDiscountValue2 { get; init; }
        public decimal? advanceDiscountPercentage2 { get; init; }
        public DateTime? advanceDiscountDate1 { get; init; }
        public DateTime? advanceDiscountDate2 { get; init; }
        public AdvanceDiscountType? advanceDiscount1 { get; init; }
        public AdvanceDiscountType? advanceDiscount2 { get; init; }
        // tipo do desconto pontual
        public AdvanceDiscountType? advanceDiscount { get; init; }
        public decimal? assessmentValue { get; init; }
        public decimal? assessmentPercentage { get; init; }
        public DateTime? assessmentDate { get; init; }
        public decimal? interestDayValue { get; init; }
        public decimal? interestDayPercentage { get; init; }
        public decimal? expenseFinancial { get; init; }
        public decimal? additionValue { get; init; }
        // juros fixo/outros valores (grafia do swagger)
        public decimal? outherAdditionalsValue { get; init; }
        public DateTime? supplierOutputDate { get; init; }
        public decimal? indemnityValue { get; init; }
        public bool? isAccumulatedDiscount { get; init; }
        public decimal? taxValue { get; init; }
        public decimal? taxCalculationBasePercentage { get; init; }

        public List<Expense>? expenses { get; init; }
        public List<ObservationInstallment>? observations { get; init; }
        public BillingAccept? billingAccept { get; init; }
        public List<DuplicateClassification>? classifications { get; init; }
        public List<DuplicateField>? additionalFields { get; init; }
        public TaxAdditional? taxAdditional { get; init; }
        public List<TaxDuplicate>? taxDuplicates { get; init; }
        public List<BusinessUnit>? businessUnits { get; init; }
    }
}
