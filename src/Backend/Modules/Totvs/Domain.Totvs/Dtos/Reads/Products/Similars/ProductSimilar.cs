namespace Domain.Totvs.Dtos.Reads.Products.Similars
{
    /// <summary>
    /// produtos similares de um produto (GET product/v2/similar, ProductSimilarModel). lastChageDate (grafia da API) vem sempre 0001-01-01 (testado) e não é gravado
    /// </summary>
    public class ProductSimilar
    {
        public string? productCode { get; init; }
        public string? groupSequenceCode { get; init; }
        public string? lastChageDate { get; init; }
        public List<SimilarProductCode>? similarProductList { get; init; }
        public List<SimilarProductCode>? similarGroupProductList { get; init; }
        public List<SimilarProductCode>? similarRefereceGroupProductList { get; init; }
    }
}
