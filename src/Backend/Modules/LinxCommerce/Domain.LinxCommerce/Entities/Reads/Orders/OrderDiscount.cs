using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// desconto do pedido, tabela linxcommerce.LinxCommerceOrdersDiscounts
    /// </summary>
    public class OrderDiscount
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public int DiscountID { get; private set; }
        public decimal? Amount { get; private set; }
        public string Message { get; private set; } = String.Empty;
        public string Reference { get; private set; } = String.Empty;
        public string Type { get; private set; } = String.Empty;

        public OrderDiscount() { }

        public OrderDiscount(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderDiscount dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            DiscountID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.DiscountID);
            Amount = NullableConverters.ToNullableDecimal(dto.Amount);
            Message = ConverterValidatorsExtensions.StringLengthValidation(dto.Message, 400);
            Reference = ConverterValidatorsExtensions.StringLengthValidation(dto.Reference, 250);
            Type = ConverterValidatorsExtensions.StringLengthValidation(dto.Type, 50);
        }
    }
}
