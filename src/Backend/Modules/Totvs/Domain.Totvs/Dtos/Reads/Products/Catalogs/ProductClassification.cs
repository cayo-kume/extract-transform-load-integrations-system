namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// classificação de produto e o seu tipo (GET product/v2/classifications, ClassificationDateModel)
    /// </summary>
    public class ProductClassification
    {
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? typeNameAux { get; init; }
        public string? code { get; init; }
        public string? name { get; init; }
        public string? nameAux { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
