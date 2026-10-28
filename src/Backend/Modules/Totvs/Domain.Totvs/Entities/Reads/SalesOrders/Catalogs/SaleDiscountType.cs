using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// tipo de desconto do pedido de venda (GET sales-order/v2/discount-type, DiscountTypeModel), tabela totvs.TotvsSaleDiscountTypes
    /// </summary>
    public class SaleDiscountType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public decimal? percentage { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public SaleDiscountType() { }

        public SaleDiscountType(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesOrders.Catalogs.SaleDiscountType dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 100);
            percentage = NullableConverters.ToNullableDecimal(dto.percentage);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
