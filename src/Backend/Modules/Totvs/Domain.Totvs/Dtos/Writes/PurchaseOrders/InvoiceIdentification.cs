namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// nota do vínculo (InvoiceIdentificationModel): pela chave de acesso ou pela série e número
    /// </summary>
    public class InvoiceIdentification
    {
        public string? accessKey { get; init; }
        public string? serialCode { get; init; }
        public long? invoiceCode { get; init; }
    }
}
