namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (cardInformation); vira colunas do pai com o prefixo cardInformation
    /// </summary>
    public class CardInformation
    {
        public string? authorizationCode { get; init; }
        public string? nsu { get; init; }
        public string? inclusionType { get; init; }
        public string? cardOperatorCode { get; init; }
        public string? cardOperatorName { get; init; }
        public string? cardFlag { get; init; }
        public string? cardFlagCode { get; init; }
    }
}
