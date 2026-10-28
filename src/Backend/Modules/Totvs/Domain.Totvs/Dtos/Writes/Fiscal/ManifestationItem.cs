namespace Domain.Totvs.Dtos.Writes.Fiscal
{
    /// <summary>
    /// documento do painel de manifestação (ManifestationModel): os dados do resumo que a SEFAZ entrega na distribuição de DF-e, informados por
    /// quem chama. operationType e invoiceStatus vão pelo nome (string no swagger); o XML não é gravado em totvs.TotvsRequests, só o tamanho
    /// </summary>
    public class ManifestationItem
    {
        public string? accessKey { get; init; }
        public string? branchCnpj { get; init; }
        public string? issuerCnpj { get; init; }
        public string? issuerCorporateName { get; init; }
        public int? invoiceCode { get; init; }
        public DateTime? issueDate { get; init; }
        public string? serialCode { get; init; }
        public long? nsu { get; init; }
        // Input (entrada) ou Output (saída)
        public string? operationType { get; init; }
        // Authorized, Canceled, Denied, Issued, Generated ou Rejected
        public string? invoiceStatus { get; init; }
        public decimal? totalValue { get; init; }
        public string? invoiceXml { get; init; }
        public ManifestationDocumentType? documentType { get; init; }
    }
}
