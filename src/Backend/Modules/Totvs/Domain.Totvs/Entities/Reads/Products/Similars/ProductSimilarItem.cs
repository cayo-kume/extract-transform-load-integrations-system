using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Similars
{
    /// <summary>
    /// produto similar, tabela totvs.TotvsProductsSimilarItems
    /// </summary>
    public class ProductSimilarItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int productCode { get; private set; }
        public string listType { get; private set; } = String.Empty;
        public int similarProductCode { get; private set; }

        public ProductSimilarItem() { }

        public ProductSimilarItem(string doc_company, DateTime integrated_or_updated_at, int productCode, string listType, Dtos.Reads.Products.Similars.SimilarProductCode dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.productCode = productCode;
            this.listType = listType;

            similarProductCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
        }
    }
}
