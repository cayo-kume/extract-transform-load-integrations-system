namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// fornecedor do SKU (ProductSupplierInsertModel)
    /// </summary>
    public class ProductSupplier
    {
        public string? supplierCpfCnpj { get; init; }
        public string? originalCode { get; init; }
        public bool? isMainSupplier { get; init; }
    }
}
