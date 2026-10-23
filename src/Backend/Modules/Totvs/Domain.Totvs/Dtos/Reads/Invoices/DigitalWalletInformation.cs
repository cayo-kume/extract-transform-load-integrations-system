namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (digitalWalletInformation); vira colunas do pai com o prefixo digitalWalletInformation
    /// </summary>
    public class DigitalWalletInformation
    {
        public string? nsu { get; init; }
        public string? transactionId { get; init; }
        public string? processorId { get; init; }
    }
}
