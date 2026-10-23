using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.PurchaseOrders
{
    /// <summary>
    /// item (produto) do pedido de compra, tabela totvs.TotvsPurchaseOrdersProducts
    /// </summary>
    public class Product
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int productCode { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string referenceCode { get; private set; } = String.Empty;
        public string referenceName { get; private set; } = String.Empty;
        public string productSku { get; private set; } = String.Empty;
        public string colorCode { get; private set; } = String.Empty;
        public string colorName { get; private set; } = String.Empty;
        public string sizeName { get; private set; } = String.Empty;
        public string measuredUnit { get; private set; } = String.Empty;
        public decimal? quantity { get; private set; }
        public decimal? lowQuantity { get; private set; }
        public decimal? settledQuantity { get; private set; }
        public decimal? canceledQuantity { get; private set; }
        public decimal? extraQuantity { get; private set; }
        public DateTime? deliveryForecastDate { get; private set; }
        public DateTime? deliveryDeadlineDate { get; private set; }
        public decimal? value { get; private set; }
        public decimal? discountPercentage { get; private set; }
        public string conversionUnit { get; private set; } = String.Empty;
        public decimal? conversionQuantity { get; private set; }

        public Product() { }

        public Product(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.PurchaseOrders.Product dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
            referenceCode = ConverterValidatorsExtensions.StringLengthValidation(dto.referenceCode, 60);
            referenceName = ConverterValidatorsExtensions.StringLengthValidation(dto.referenceName, 100);
            productSku = ConverterValidatorsExtensions.StringLengthValidation(dto.productSku, 60);
            colorCode = ConverterValidatorsExtensions.StringLengthValidation(dto.colorCode, 20);
            colorName = ConverterValidatorsExtensions.StringLengthValidation(dto.colorName, 60);
            sizeName = ConverterValidatorsExtensions.StringLengthValidation(dto.sizeName, 20);
            measuredUnit = ConverterValidatorsExtensions.StringLengthValidation(dto.measuredUnit, 10);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            lowQuantity = NullableConverters.ToNullableDecimal(dto.lowQuantity);
            settledQuantity = NullableConverters.ToNullableDecimal(dto.settledQuantity);
            canceledQuantity = NullableConverters.ToNullableDecimal(dto.canceledQuantity);
            extraQuantity = NullableConverters.ToNullableDecimal(dto.extraQuantity);
            deliveryForecastDate = NullableConverters.ToNullableDateTime(dto.deliveryForecastDate);
            deliveryDeadlineDate = NullableConverters.ToNullableDateTime(dto.deliveryDeadlineDate);
            value = NullableConverters.ToNullableDecimal(dto.value);
            discountPercentage = NullableConverters.ToNullableDecimal(dto.discountPercentage);
            conversionUnit = ConverterValidatorsExtensions.StringLengthValidation(dto.conversionUnit, 10);
            conversionQuantity = NullableConverters.ToNullableDecimal(dto.conversionQuantity);
        }
    }
}
