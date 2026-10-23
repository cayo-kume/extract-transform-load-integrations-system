namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// detalhe da referência (ProductDetailModel); typeCode e auxiliaryType se repetem na mesma referência
    /// </summary>
    public class Detail
    {
        public string? typeCode { get; init; }
        public string? type { get; init; }
        public string? auxiliaryType { get; init; }
        public string? title { get; init; }
        public string? description { get; init; }
    }
}
