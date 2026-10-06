using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Discount
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }
        
        public int typeDiscountCode { get; private set; }

        public decimal discountPercentage { get; private set; }
        public decimal discountValue { get; private set; }
        
        public string description { get; private set; }

        public Discount() { }

        public Discount(Domain.Totvs.Dtos.SalesOrder.Discount discountDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            typeDiscountCode = ConverterValidatorsExtensions.ConvertToInt32Validation(discountDto.typeDiscountCode);
            discountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(discountDto.discountPercentage);
            discountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(discountDto.discountValue);

            description = discountDto.description?.Length > 60 ? discountDto.description?.Substring(0, 60) : discountDto.description;
            this.branchCode = branchCode;
        }
    }
}
