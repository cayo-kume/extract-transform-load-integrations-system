namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// fornecedor do produto (SupplierModel)
    /// </summary>
    public class Supplier
    {
        public string? code { get; init; }
        public string? cpfCnpj { get; init; }
        public string? name { get; init; }
        public string? originalCode { get; init; }
        public string? isDefault { get; init; }
    }
}
