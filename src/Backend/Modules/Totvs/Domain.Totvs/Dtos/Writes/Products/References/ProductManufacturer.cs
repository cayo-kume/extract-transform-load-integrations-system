namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// fabricante do SKU (ProductManufacturerInsertModel)
    /// </summary>
    public class ProductManufacturer
    {
        public string? manufacturerCpfCnpj { get; init; }
        public string? originalCode { get; init; }
    }
}
