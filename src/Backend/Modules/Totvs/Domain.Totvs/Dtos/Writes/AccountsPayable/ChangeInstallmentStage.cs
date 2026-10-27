namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// corpo do POST accounts-payable/v2/duplicates/installments/stage (ChangeInstallmentStageCommand): troca o estágio de uma
    /// parcela. Recebido pela rota do controller e enviado à Totvs como está; nulos não são enviados
    /// </summary>
    public class ChangeInstallmentStage
    {
        // CNPJ da loja da duplicata (14 dígitos); precisa ser uma loja da empresa em totvs.TotvsBranches
        public string? branchCnpj { get; init; }

        // fornecedor: exatamente um dos dois (a Totvs recusa os dois juntos)
        public string? supplierCpfCnpj { get; init; }
        public int? supplierCode { get; init; }

        public long? duplicateCode { get; init; }
        public int? installmentCode { get; init; }

        public ChangeInstallmentStageType? newStage { get; init; }
    }
}
