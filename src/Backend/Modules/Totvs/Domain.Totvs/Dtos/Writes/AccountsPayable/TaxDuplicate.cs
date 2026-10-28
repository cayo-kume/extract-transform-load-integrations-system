namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// imposto da duplicata (TaxDuplicateModel); suplierCpfCnpj é a grafia da API
    /// </summary>
    public class TaxDuplicate
    {
        public int? tax { get; init; }
        public string? branchCnpj { get; init; }
        public string? suplierCpfCnpj { get; init; }
        public long? duplicateCodeTax { get; init; }
        public int? installmentCodeTax { get; init; }
        // 3 inteiros e 6 decimais
        public decimal? ratePercentage { get; init; }
        public decimal? taxValue { get; init; }
        public StatusTaxDuplicateType? status { get; init; }
        public decimal? baseCalculationValue { get; init; }
        public decimal? baseCalculationPercentage { get; init; }
        public RetentionType? retentionType { get; init; }
        public string? dirfRevenue { get; init; }
    }
}
