using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// item (produto) do pedido de venda, tabela totvs.TotvsSaleOrdersProducts
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
        public decimal? quantity { get; private set; }
        public decimal? toSettleQuantity { get; private set; }
        public decimal? settledQuantity { get; private set; }
        public decimal? canceledQuantity { get; private set; }
        public decimal? extraQuantity { get; private set; }
        public decimal? pendingQuantity { get; private set; }
        public decimal? originalPrice { get; private set; }
        public decimal? price { get; private set; }
        public decimal? discountPercentage { get; private set; }
        public int? lastChangeUserInsert { get; private set; }
        public DateTime? lastChangeDate { get; private set; }
        public DateTime? billingForecastDate { get; private set; }
        public DateTime? insertDate { get; private set; }
        public DateTime? cancellationLimitDate { get; private set; }
        public DateTime? billingLimitDate { get; private set; }
        public DateTime? lastBillingDate { get; private set; }
        public int? rollMeasureCode { get; private set; }
        public string rollMeasureDescription { get; private set; } = String.Empty;
        public int? customerOrderCode { get; private set; }
        public string customerProductCode { get; private set; } = String.Empty;

        [SkipProperty]
        public List<BatchItem> batchItems { get; private set; } = [];

        public Product() { }

        public Product(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Product dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            referenceCode = ConverterValidatorsExtensions.StringLengthValidation(dto.referenceCode, 60);
            referenceName = ConverterValidatorsExtensions.StringLengthValidation(dto.referenceName, 100);
            productSku = ConverterValidatorsExtensions.StringLengthValidation(dto.productSku, 40);
            colorCode = ConverterValidatorsExtensions.StringLengthValidation(dto.colorCode, 10);
            colorName = ConverterValidatorsExtensions.StringLengthValidation(dto.colorName, 40);
            sizeName = ConverterValidatorsExtensions.StringLengthValidation(dto.sizeName, 10);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            toSettleQuantity = NullableConverters.ToNullableDecimal(dto.toSettleQuantity);
            settledQuantity = NullableConverters.ToNullableDecimal(dto.settledQuantity);
            canceledQuantity = NullableConverters.ToNullableDecimal(dto.canceledQuantity);
            extraQuantity = NullableConverters.ToNullableDecimal(dto.extraQuantity);
            pendingQuantity = NullableConverters.ToNullableDecimal(dto.pendingQuantity);
            originalPrice = NullableConverters.ToNullableDecimal(dto.originalPrice);
            price = NullableConverters.ToNullableDecimal(dto.price);
            discountPercentage = NullableConverters.ToNullableDecimal(dto.discountPercentage);
            lastChangeUserInsert = NullableConverters.ToNullableInt32(dto.lastChangeUserInsert);
            lastChangeDate = NullableConverters.ToNullableDateTime(dto.lastChangeDate);
            billingForecastDate = NullableConverters.ToNullableDateTime(dto.billingForecastDate);
            insertDate = NullableConverters.ToNullableDateTime(dto.insertDate);
            cancellationLimitDate = NullableConverters.ToNullableDateTime(dto.cancellationLimitDate);
            billingLimitDate = NullableConverters.ToNullableDateTime(dto.billingLimitDate);
            lastBillingDate = NullableConverters.ToNullableDateTime(dto.lastBillingDate);
            rollMeasureCode = NullableConverters.ToNullableInt32(dto.rollMeasureCode);
            rollMeasureDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.rollMeasureDescription, 80);
            customerOrderCode = NullableConverters.ToNullableInt32(dto.customerOrderCode);
            customerProductCode = ConverterValidatorsExtensions.StringLengthValidation(dto.customerProductCode, 20);

            // repetidos violariam a PK
            batchItems.AddRange((dto.batchItems ?? []).Select(x => new BatchItem(doc_company, integrated_or_updated_at, branchCode, orderCode, productCode, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
        }
    }
}
