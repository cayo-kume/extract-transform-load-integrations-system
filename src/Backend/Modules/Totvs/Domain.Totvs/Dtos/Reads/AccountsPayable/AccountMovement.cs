namespace Domain.Totvs.Dtos.Reads.AccountsPayable
{
    /// <summary>
    /// movimento da conta do comissionado (sem chave na API: a posição é a chave)
    /// </summary>
    public class AccountMovement
    {
        public string? movementDate { get; init; }
        public string? auxiliaryHistoryDescription { get; init; }
        public string? documentDescription { get; init; }
        public string? historyCode { get; init; }
        public string? historyDescription { get; init; }
        public string? launchValue { get; init; }
    }
}
