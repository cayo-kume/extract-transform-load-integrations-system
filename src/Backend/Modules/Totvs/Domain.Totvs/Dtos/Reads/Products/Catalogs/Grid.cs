namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// grade de tamanhos (GET product/v2/grid, GridModel)
    /// </summary>
    public class Grid
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? type { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<string>? grid { get; init; }
    }
}
