namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (rateDifferential); vira colunas do pai com o prefixo rateDifferential
    /// </summary>
    public class RateDifferential
    {
        public string? calculationBaseValue { get; init; }
        public string? internalRate { get; init; }
        public string? interstateRate { get; init; }
        public string? fcpRate { get; init; }
        public string? fcpValue { get; init; }
        public string? recipientRateDifferentialValue { get; init; }
        public string? senderRateDifferentialValue { get; init; }
    }
}
