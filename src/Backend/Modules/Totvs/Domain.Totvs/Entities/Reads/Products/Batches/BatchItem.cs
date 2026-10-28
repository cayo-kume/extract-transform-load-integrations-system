using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Batches
{
    /// <summary>
    /// item do lote, tabela totvs.TotvsBatchesItems
    /// </summary>
    public class BatchItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int batchCode { get; private set; }
        public int productCode { get; private set; }
        public int batchItemSequential { get; private set; }
        public decimal? balance { get; private set; }
        public decimal? quantityItem { get; private set; }
        public decimal? quantityIssued { get; private set; }
        public decimal? quantityTare { get; private set; }
        public decimal? quantityWeigth { get; private set; }
        public decimal? quantityMeters { get; private set; }
        public decimal? quantityCanceled { get; private set; }
        public decimal? quantityYield { get; private set; }
        public string itemProvider { get; private set; } = String.Empty;
        public string itemClient { get; private set; } = String.Empty;
        public string locationBatch { get; private set; } = String.Empty;
        public string status { get; private set; } = String.Empty;
        public string conditioningType { get; private set; } = String.Empty;
        public string nuance { get; private set; } = String.Empty;

        public BatchItem() { }

        public BatchItem(string doc_company, DateTime integrated_or_updated_at, int branchCode, int batchCode, int productCode, Dtos.Reads.Products.Batches.BatchItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.batchCode = batchCode;
            this.productCode = productCode;

            batchItemSequential = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.batchItemSequential);
            balance = NullableConverters.ToNullableDecimal(dto.balance);
            quantityItem = NullableConverters.ToNullableDecimal(dto.quantityItem);
            quantityIssued = NullableConverters.ToNullableDecimal(dto.quantityIssued);
            quantityTare = NullableConverters.ToNullableDecimal(dto.quantityTare);
            quantityWeigth = NullableConverters.ToNullableDecimal(dto.quantityWeigth);
            quantityMeters = NullableConverters.ToNullableDecimal(dto.quantityMeters);
            quantityCanceled = NullableConverters.ToNullableDecimal(dto.quantityCanceled);
            quantityYield = NullableConverters.ToNullableDecimal(dto.quantityYield);
            itemProvider = ConverterValidatorsExtensions.StringLengthValidation(dto.itemProvider, 40);
            itemClient = ConverterValidatorsExtensions.StringLengthValidation(dto.itemClient, 40);
            locationBatch = ConverterValidatorsExtensions.StringLengthValidation(dto.locationBatch, 5);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 30);
            conditioningType = ConverterValidatorsExtensions.StringLengthValidation(dto.conditioningType, 10);
            nuance = ConverterValidatorsExtensions.StringLengthValidation(dto.nuance, 20);
        }
    }
}
