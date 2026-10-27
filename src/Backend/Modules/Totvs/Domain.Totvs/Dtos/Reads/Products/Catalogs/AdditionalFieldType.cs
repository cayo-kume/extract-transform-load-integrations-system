namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// tipo de campo adicional de produto (GET product/v2/additional-fields-types; o type vem como número, 1 a 3, e não pelo nome do enum, testado)
    /// </summary>
    public class AdditionalFieldType
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? title { get; init; }
        public string? type { get; init; }
        public string? size { get; init; }
        public string? @decimal { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
