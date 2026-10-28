namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// corpo do POST accounts-payable/v2/duplicates (CreatedDuplicateCommand): inclui uma duplicata a pagar com as parcelas. Recebido pela
    /// rota do controller e enviado à Totvs como está; nulos não são enviados. A duplicata chega ao banco pelo próximo PostSearchDuplicates (D3)
    /// </summary>
    public class Duplicate
    {
        // CNPJ da loja que grava a duplicata (14 dígitos); precisa ser uma loja da empresa em totvs.TotvsBranches
        public string? branchCnpj { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public long? duplicateCode { get; init; }
        public List<Installment>? installments { get; init; }
    }
}
