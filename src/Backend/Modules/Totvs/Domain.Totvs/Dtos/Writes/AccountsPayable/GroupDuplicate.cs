namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// corpo do POST accounts-payable/v2/group-duplicates (GroupDuplicateCommand): agrupa parcelas numa duplicata nova (a chave do
    /// corpo). A resposta traz loja, fornecedor, duplicata e parcela gerados; tudo chega ao banco pelo próximo PostSearchDuplicates
    /// </summary>
    public class GroupDuplicate
    {
        public string? branchCnpj { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public long? duplicateCode { get; init; }
        public GroupDocumentType? document { get; init; }
        public DateTime? dueDate { get; init; }
        public List<GroupInstallment>? groupInstallments { get; init; }
    }
}
