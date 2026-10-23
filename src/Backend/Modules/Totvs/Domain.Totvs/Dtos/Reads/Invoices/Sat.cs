namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (sat); vira colunas da nota com o prefixo sat
    /// </summary>
    public class Sat
    {
        public string? satCode { get; init; }
        public string? dvNumber { get; init; }
        public string? cnfNumber { get; init; }
        public string? cancellationKey { get; init; }
        public string? satSequence { get; init; }
        public string? serialMachine { get; init; }
    }
}
