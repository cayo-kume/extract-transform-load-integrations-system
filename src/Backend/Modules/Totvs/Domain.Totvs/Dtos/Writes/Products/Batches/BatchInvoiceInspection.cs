namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// nota e inspeção do item de lote (BatchInvoiceInspectionModel): fornecedor pelo código ou pelo CPF/CNPJ
    /// </summary>
    public class BatchInvoiceInspection
    {
        public int? supplierCode { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public int? invoiceCode { get; init; }
        public int? inspectionLayout { get; init; }
        public int? quantityInspection { get; init; }
        public List<BatchInspectionField>? inspectionFields { get; init; }
    }
}
