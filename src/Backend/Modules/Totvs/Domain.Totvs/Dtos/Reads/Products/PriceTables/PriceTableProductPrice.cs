namespace Domain.Totvs.Dtos.Reads.Products.PriceTables
{
    /// <summary>
    /// preço do produto na tabela, por loja e escala (PriceTableItemModel)
    /// </summary>
    public class PriceTableProductPrice
    {
        public string? branchCode { get; init; }
        public string? originalPrice { get; init; }
        public string? price { get; init; }
        public string? scaleCode { get; init; }
    }
}
