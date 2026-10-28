namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// parcela que entra no agrupamento (GroupDuplicateModel)
    /// </summary>
    public class GroupInstallment
    {
        public string? branchCnpj { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public long? duplicateCode { get; init; }
        public int? installmentCode { get; init; }
    }
}
