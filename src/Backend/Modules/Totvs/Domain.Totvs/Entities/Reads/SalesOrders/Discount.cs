using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// desconto do pedido, tabela totvs.TotvsSaleOrdersDiscounts
    /// </summary>
    public class Discount
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int typeDiscountCode { get; private set; }
        public string description { get; private set; } = String.Empty;
        public decimal? discountPercentage { get; private set; }
        public decimal? discountValue { get; private set; }

        public Discount() { }

        public Discount(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Discount dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            typeDiscountCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeDiscountCode);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            discountPercentage = NullableConverters.ToNullableDecimal(dto.discountPercentage);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
        }
    }
}
