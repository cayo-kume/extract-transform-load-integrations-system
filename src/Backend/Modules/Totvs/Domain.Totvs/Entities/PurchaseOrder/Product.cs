using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.PurchaseOrder
{
    public class Product
    {
        public int purchaseOrderBranchCode { get; private set; }
        public int purchaseOrderCode { get; private set; }

        public int productCode { get; private set; }
        public int quantity { get; private set; }
        public int settledQuantity { get; private set; }
        public int lowQuantity { get; private set; }
        public int canceledQuantity { get; private set; }
        public int extraQuantity { get; private set; }
        public int conversionQuantity { get; private set; }
        
        public decimal discountPercentage { get; private set; }
        public decimal value { get; private set; }
        
        public string name { get; private set; }
        public string referenceCode { get; private set; }
        public string referenceName { get; private set; }
        public string productSku { get; private set; }
        public string colorCode { get; private set; }
        public string colorName { get; private set; }
        public string sizeName { get; private set; }
        public string measuredUnit { get; private set; }
        public string conversionUnit { get; private set; }
        
        public DateTime deliveryForecastDate { get; private set; }
        public DateTime deliveryDeadlineDate { get; private set; }

        public Product() { }

        public Product(Totvs.Dtos.PurchaseOrder.Product productDto, int purchaseOrderCode, int purchaseOrderBranchCode)
        {
            this.purchaseOrderBranchCode = purchaseOrderBranchCode;
            this.purchaseOrderCode = purchaseOrderCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.productCode);
            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.quantity);
            lowQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.lowQuantity);
            settledQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.settledQuantity);
            canceledQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.canceledQuantity);
            extraQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.extraQuantity);
            conversionQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.conversionQuantity);
            
            discountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.discountPercentage);
            value = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.value);

            deliveryForecastDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.deliveryForecastDate);
            deliveryDeadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.deliveryDeadlineDate);

            name = productDto.name?.Length > 250 ? productDto.name?.Substring(0, 250) : productDto.name;
            referenceCode = productDto.referenceCode?.Length > 60 ? productDto.referenceCode?.Substring(0, 60) : productDto.referenceCode;
            referenceName = productDto.referenceName?.Length > 250 ? productDto.referenceName?.Substring(0, 250) : productDto.referenceName;
            productSku = productDto.productSku?.Length > 60 ? productDto.productSku?.Substring(0, 60) : productDto.productSku;
            colorCode = productDto.colorCode?.Length > 60 ? productDto.colorCode?.Substring(0, 60) : productDto.colorCode;
            colorName = productDto.colorName?.Length > 60 ? productDto.colorName?.Substring(0, 60) : productDto.colorName;
            sizeName = productDto.sizeName?.Length > 60 ? productDto.sizeName?.Substring(0, 60) : productDto.sizeName;
            measuredUnit = productDto.measuredUnit?.Length > 660 ? productDto.measuredUnit?.Substring(0, 60) : productDto.measuredUnit;
            conversionUnit = productDto.conversionUnit?.Length > 60 ? productDto.conversionUnit?.Substring(0, 60) : productDto.conversionUnit;
        }
    }
}
