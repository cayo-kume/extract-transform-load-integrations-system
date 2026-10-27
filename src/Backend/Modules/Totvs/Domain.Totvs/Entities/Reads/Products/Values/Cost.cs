using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// custo de um produto numa loja, por tipo de custo (PRDFL003), tabela totvs.TotvsCosts
    /// </summary>
    public class Cost
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int productCode { get; private set; }
        public int costCode { get; private set; }
        public string costName { get; private set; } = String.Empty;
        public decimal? cost { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public Cost() { }

        public Cost(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Values.ProductCost product, Dtos.Reads.Products.Values.ProductCostItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(product.productCode);
            costCode = (int)(NullableConverters.ToNullableDecimal(dto.costCode) ?? 0);
            costName = ConverterValidatorsExtensions.StringLengthValidation(dto.costName, 60);
            cost = NullableConverters.ToNullableDecimal(dto.cost);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(product.maxChangeFilterDate);
        }
    }
}
