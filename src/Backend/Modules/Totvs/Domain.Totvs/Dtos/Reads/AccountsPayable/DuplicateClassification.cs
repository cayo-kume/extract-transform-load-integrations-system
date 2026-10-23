namespace Domain.Totvs.Dtos.Reads.AccountsPayable
{
    /// <summary>
    /// classificação da duplicata (só vem com expand=classifications; classificationCode nulo vira vazio, faz parte da chave)
    /// </summary>
    public class DuplicateClassification
    {
        public string? classificationType { get; init; }
        public string? classificationCode { get; init; }
        public string? classificationTypeDescription { get; init; }
        public string? classificationDescription { get; init; }
    }
}
