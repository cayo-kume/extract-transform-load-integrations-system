namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// item da configuração de agrupador (ProductGrouperItemConfigurationModel)
    /// </summary>
    public class ProductGrouperConfigurationItem
    {
        public string? code { get; init; }
        public string? order { get; init; }
        public string? description { get; init; }
        public string? isMandatory { get; init; }
    }
}
