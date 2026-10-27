namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// tipo de classificação de produto (GET product/v2/classificationType, ClassificationTypeResultModel)
    /// </summary>
    public class ClassificationType
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? auxiliaryDescription { get; init; }
        public string? isGroup { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
