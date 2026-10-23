namespace Domain.Totvs.Dtos.Reads.AccountsPayable
{
    /// <summary>
    /// duplicata (parcela) a pagar do POST accounts-payable/v2/duplicates/search
    /// </summary>
    public class Duplicate
    {
        public string? maxChangeFilterDate { get; init; }
        public string? branchCode { get; init; }
        public string? duplicateCode { get; init; }
        public string? supplierCode { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public string? installmentCode { get; init; }
        public string? bearerCode { get; init; }
        public string? entryDate { get; init; }
        public string? issueDate { get; init; }
        public string? dueDate { get; init; }
        public string? settlementDate { get; init; }
        public string? arrivalDate { get; init; }
        public string? status { get; init; }
        public string? duplicateValue { get; init; }
        public string? feesValue { get; init; }
        public string? discountValue { get; init; }
        public string? paidValue { get; init; }
        public string? inclusionType { get; init; }
        public string? userInclusionCode { get; init; }
        public string? userInclusionName { get; init; }

        public List<Expense>? expense { get; init; }
        public List<DuplicateClassification>? classifications { get; init; }
    }
}
