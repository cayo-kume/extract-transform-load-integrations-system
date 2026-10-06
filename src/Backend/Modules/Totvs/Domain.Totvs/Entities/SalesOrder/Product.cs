using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Product
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }

        public int productCode { get; private set; }
        public int quantity { get; private set; }
        public int toSettleQuantity { get; private set; }
        public int settledQuantity { get; private set; }
        public int canceledQuantity { get; private set; }
        public int extraQuantity { get; private set; }
        public int pendingQuantity { get; private set; }
        public decimal originalPrice { get; private set; }
        public decimal price { get; private set; }
        public decimal discountPercentage { get; private set; }
        public int lastChangeUserInsert { get; private set; }
        public int rollMeasureCode { get; private set; }
        public int customerOrderCode { get; private set; }
        
        public DateTime lastChangeDate { get; private set; }
        public DateTime billingForecastDate { get; private set; }
        public DateTime insertDate { get; private set; }
        public DateTime cancellationLimitDate { get; private set; }
        public DateTime billingLimitDate { get; private set; }
        public DateTime lastBillingDate { get; private set; }
        
        public string name { get; private set; }
        public string referenceCode { get; private set; }
        public string referenceName { get; private set; }
        public string productSku { get; private set; }
        public string colorCode { get; private set; }
        public string colorName { get; private set; }
        public string sizeName { get; private set; }
        public string rollMeasureDescription { get; private set; }
        public string customerProductCode { get; private set; }

        [SkipProperty]
        public List<BatchItem> batchItems { get; private set; } = new List<BatchItem>();

        public Product() { }

        public Product(Domain.Totvs.Dtos.SalesOrder.Product productDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.productCode);
            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.quantity);
            toSettleQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.toSettleQuantity);
            settledQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.settledQuantity);
            canceledQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.canceledQuantity);
            extraQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.extraQuantity);
            pendingQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.pendingQuantity);
            originalPrice = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.originalPrice);
            price = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.price);
            discountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.discountPercentage);
            lastChangeUserInsert = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.lastChangeUserInsert);
            rollMeasureCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.rollMeasureCode);
            customerOrderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.customerOrderCode);

            lastChangeDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.lastChangeDate);
            billingForecastDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.billingForecastDate);
            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.insertDate);
            cancellationLimitDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.cancellationLimitDate);
            billingLimitDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.billingLimitDate);
            lastBillingDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.lastBillingDate);

            name = productDto.name?.Length > 60 ? productDto.name?.Substring(0, 60) : productDto.name;
            referenceCode = productDto.referenceCode?.Length > 60 ? productDto.referenceCode?.Substring(0, 60) : productDto.referenceCode;
            referenceName = productDto.referenceName?.Length > 60 ? productDto.referenceName?.Substring(0, 60) : productDto.referenceName;
            productSku = productDto.productSku?.Length > 60 ? productDto.productSku?.Substring(0, 60) : productDto.productSku;
            colorCode = productDto.colorCode?.Length > 60 ? productDto.colorCode?.Substring(0, 60) : productDto.colorCode;
            colorName = productDto.colorName?.Length > 60 ? productDto.colorName?.Substring(0, 60) : productDto.colorName;
            sizeName = productDto.sizeName?.Length > 60 ? productDto.sizeName?.Substring(0, 60) : productDto.sizeName;
            rollMeasureDescription = productDto.rollMeasureDescription?.Length > 60 ? productDto.rollMeasureDescription?.Substring(0, 60) : productDto.rollMeasureDescription;
            customerProductCode = productDto.customerProductCode?.Length > 60 ? productDto.customerProductCode?.Substring(0, 60) : productDto.customerProductCode;

            productDto.batchItems?.ForEach(batchItem =>
                batchItems.Add(new BatchItem(batchItem, productCode, saleOrderCode)
            ));
            this.branchCode = branchCode;
        }
    }
}
