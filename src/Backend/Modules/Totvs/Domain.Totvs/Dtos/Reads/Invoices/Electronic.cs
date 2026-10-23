namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (eletronic); vira colunas da nota com o prefixo eletronic
    /// </summary>
    public class Electronic
    {
        public string? accessKey { get; init; }
        public string? electronicInvoiceStatus { get; init; }
        public string? receipt { get; init; }
        public string? receivementDate { get; init; }
        public string? disableProtocol { get; init; }
        public string? disableDate { get; init; }
        public string? nfseCode { get; init; }
    }
}
