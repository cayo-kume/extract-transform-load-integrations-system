namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// composição do cadastro mestre (POST product/v2/compositions, CompositionResultModel)
    /// </summary>
    public class Composition
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<CompositionFiber>? fibers { get; init; }
    }
}
