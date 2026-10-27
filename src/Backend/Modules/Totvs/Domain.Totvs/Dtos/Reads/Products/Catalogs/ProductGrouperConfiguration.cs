namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// configuração de agrupador de produto (GET product/v2/product-grouper-configuration, ProductGrouperConfigurationResultModel)
    /// </summary>
    public class ProductGrouperConfiguration
    {
        public string? grouperCode { get; init; }
        public string? description { get; init; }
        public string? type { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<ProductGrouperConfigurationItem>? productGrouperItemConfigurationList { get; init; }
    }
}
