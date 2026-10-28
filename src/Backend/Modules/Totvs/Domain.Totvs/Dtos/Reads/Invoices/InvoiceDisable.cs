namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// faixa de numeração inutilizada na SEFAZ (GET fiscal/v2/invoices/disable, InvoiceDisableDataModel; componente FISFC012)
    /// </summary>
    public class InvoiceDisable
    {
        public string? branchCode { get; init; }
        public string? branchCnpj { get; init; }
        public string? initialInvoiceNumber { get; init; }
        public string? finalInvoiceNumber { get; init; }
        public string? serialCode { get; init; }
        public string? documentType { get; init; }
        public string? receipt { get; init; }
        public string? receivementDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
