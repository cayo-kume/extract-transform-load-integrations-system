namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// classificação da duplicata (DuplicateClassificationModel); as descrições são as do cadastro
    /// </summary>
    public class DuplicateClassification
    {
        public int? classificationType { get; init; }
        public string? classificationTypeDescription { get; init; }
        public string? classificationCode { get; init; }
        public string? classificationDescription { get; init; }
    }
}
