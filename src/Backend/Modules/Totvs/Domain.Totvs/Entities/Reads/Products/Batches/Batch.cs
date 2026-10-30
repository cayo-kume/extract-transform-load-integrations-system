using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Batches
{
    /// <summary>
    /// lote de um produto numa loja, tabela totvs.TotvsBatches, com os itens e os campos adicionais. A resposta não tem data de
    /// alteração: json_hash (SHA-256 do JSON do lote) diz se ele mudou (T9 do src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class Batch
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int batchCode { get; private set; }
        public int productCode { get; private set; }
        public string batchProvider { get; private set; } = String.Empty;
        public string batchClient { get; private set; } = String.Empty;
        public DateTime? batchManufacturingDate { get; private set; }
        public DateTime? batchExpirationDate { get; private set; }
        public string batchType { get; private set; } = String.Empty;
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<BatchItem> items { get; private set; } = [];

        [SkipProperty]
        public List<BatchItemAdditionalField> additionalFields { get; private set; } = [];

        public Batch() { }

        public Batch(string doc_company, DateTime integrated_or_updated_at, string json_hash, Dtos.Reads.Products.Batches.Batch dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            batchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.batchCode);
            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            batchProvider = ConverterValidatorsExtensions.StringLengthValidation(dto.batchProvider, 40);
            batchClient = ConverterValidatorsExtensions.StringLengthValidation(dto.batchClient, 40);
            batchManufacturingDate = NullableConverters.ToNullableDateTime(dto.batchManufacturingDate);
            batchExpirationDate = NullableConverters.ToNullableDateTime(dto.batchExpirationDate);
            batchType = ConverterValidatorsExtensions.StringLengthValidation(dto.batchType, 30);

            // repetidos violariam a PK; item sem sequencial não tem chave
            foreach (var item in (dto.batchItem ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.batchItemSequential) != 0).GroupBy(x => x.batchItemSequential).Select(g => g.Last()))
            {
                var entity = new BatchItem(doc_company, integrated_or_updated_at, branchCode, batchCode, productCode, item);
                items.Add(entity);

                additionalFields.AddRange((item.additionalFields ?? [])
                    .Select(x => new BatchItemAdditionalField(doc_company, integrated_or_updated_at, branchCode, batchCode, productCode, entity.batchItemSequential, x))
                    .Where(x => x.code != 0)
                    .GroupBy(x => x.code)
                    .Select(g => g.Last()));
            }
        }
    }
}
