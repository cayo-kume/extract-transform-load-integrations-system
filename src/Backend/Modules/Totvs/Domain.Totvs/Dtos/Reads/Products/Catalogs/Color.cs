namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// cor do cadastro mestre (POST product/v2/colors/search, ColorDataResponseModel)
    /// </summary>
    public class Color
    {
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public List<ColorAdditionalInformation>? additionalColorInformation { get; init; }
    }
}
