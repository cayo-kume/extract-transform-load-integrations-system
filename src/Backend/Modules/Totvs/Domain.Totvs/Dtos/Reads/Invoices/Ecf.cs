namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (ecf); vira colunas da nota com o prefixo ecf
    /// </summary>
    public class Ecf
    {
        public string? coupon { get; init; }
        public string? serialMachine { get; init; }
    }
}
