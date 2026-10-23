namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// imposto do item da nota (code nulo vira 0, faz parte da chave)
    /// </summary>
    public class ItemTax
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? cst { get; init; }
        public string? taxPercentage { get; init; }
        public string? calculationBasisPercentage { get; init; }
        public string? calculationBasisDiscountPercentage { get; init; }
        public string? calculationBasisValue { get; init; }
        public string? freeValue { get; init; }
        public string? otherValue { get; init; }
        public string? taxValue { get; init; }
        public string? benefitCode { get; init; }
        public string? unencumberedValue { get; init; }
        public string? unencumberedReason { get; init; }
        public string? deferredBaseValue { get; init; }
        public string? taxClassificationCode { get; init; }
    }
}
