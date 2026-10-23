namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// fabricante do produto (ManufacturerModel)
    /// </summary>
    public class Manufacturer
    {
        public string? code { get; init; }
        public string? cpfCnpj { get; init; }
        public string? name { get; init; }
    }
}
