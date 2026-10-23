namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// campo adicional do produto (AdditionalFieldModel)
    /// </summary>
    public class AdditionalField
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? type { get; init; }
        public string? value { get; init; }
    }
}
