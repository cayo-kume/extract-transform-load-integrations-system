namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// categoria de produto (GET product/v2/category, CategoryModel)
    /// </summary>
    public class Category
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? parentCategoryCode { get; init; }
        public string? categoryType { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
