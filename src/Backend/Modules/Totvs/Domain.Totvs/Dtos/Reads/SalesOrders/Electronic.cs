namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// objeto 1:1 (electronic); vira colunas do pai com o prefixo electronic
    /// </summary>
    public class Electronic
    {
        public string? accessKey { get; init; }
        public string? electronicInvoiceStatus { get; init; }
        public string? receipt { get; init; }
        public string? receivementDate { get; init; }
    }
}
