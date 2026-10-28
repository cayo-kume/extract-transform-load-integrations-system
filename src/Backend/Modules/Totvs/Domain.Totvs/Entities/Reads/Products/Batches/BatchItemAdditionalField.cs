using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Batches
{
    /// <summary>
    /// campo adicional do item de lote, tabela totvs.TotvsBatchesItemsAdditionalFields (value aceita texto, número ou data, DD/MM/YYYY)
    /// </summary>
    public class BatchItemAdditionalField
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int batchCode { get; private set; }
        public int productCode { get; private set; }
        public int batchItemSequential { get; private set; }
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string value { get; private set; } = String.Empty;

        public BatchItemAdditionalField() { }

        public BatchItemAdditionalField(string doc_company, DateTime integrated_or_updated_at, int branchCode, int batchCode, int productCode, int batchItemSequential, Dtos.Reads.Products.Batches.BatchItemAdditionalField dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.batchCode = batchCode;
            this.productCode = productCode;
            this.batchItemSequential = batchItemSequential;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            value = ConverterValidatorsExtensions.StringLengthValidation(dto.value, 250);
        }
    }
}
