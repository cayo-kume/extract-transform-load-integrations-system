using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// cabeçalho de tabela de preço (GET product/v2/price-tables-headers, PriceTablesHeadersModel), tabela totvs.TotvsPriceTables e filhas
    /// </summary>
    public class PriceTable
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string codeName { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }
        public DateTime? startDate { get; private set; }
        public DateTime? endDate { get; private set; }
        public decimal? variationValue { get; private set; }
        public decimal? variationPercentage { get; private set; }

        [SkipProperty]
        public List<PriceTableSalesOrderClassification> salesOrderClassifications { get; private set; } = [];

        [SkipProperty]
        public List<PriceTablePersonClassification> personClassifications { get; private set; } = [];

        [SkipProperty]
        public List<PriceTablePaymentCondition> paymentConditions { get; private set; } = [];

        [SkipProperty]
        public List<PriceTableAveragePeriod> averagePeriods { get; private set; } = [];

        [SkipProperty]
        public List<PriceTableAveragePeriodQuantity> averagePeriodQuantities { get; private set; } = [];

        public PriceTable() { }

        public PriceTable(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.PriceTable dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            codeName = ConverterValidatorsExtensions.StringLengthValidation(dto.codeName, 20);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 40);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            startDate = NullableConverters.ToNullableDateTime(dto.startDate);
            endDate = NullableConverters.ToNullableDateTime(dto.endDate);
            variationValue = NullableConverters.ToNullableDecimal(dto.variationValue);
            variationPercentage = NullableConverters.ToNullableDecimal(dto.variationPercentage);

            // repetidos violariam a PK
            salesOrderClassifications.AddRange((dto.salesOrderClassification ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode) != 0 && !String.IsNullOrWhiteSpace(x.code)).Select(x => new PriceTableSalesOrderClassification(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => $"{x.typeCode}|{x.code.ToUpperInvariant()}").Select(g => g.Last()));
            personClassifications.AddRange((dto.personClassification ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode) != 0 && !String.IsNullOrWhiteSpace(x.code)).Select(x => new PriceTablePersonClassification(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => $"{x.typeCode}|{x.code.ToUpperInvariant()}").Select(g => g.Last()));
            paymentConditions.AddRange((dto.paymentConditions ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).Select(x => new PriceTablePaymentCondition(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.code).Select(g => g.Last()));
            averagePeriods.AddRange((dto.averagePeriod ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.quantity) != 0).Select(x => new PriceTableAveragePeriod(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.quantity).Select(g => g.Last()));
            // a posição é a chave: o item não tem código
            averagePeriodQuantities.AddRange((dto.averagePeriodQuantity ?? []).Select((x, i) => new PriceTableAveragePeriodQuantity(doc_company, integrated_or_updated_at, code, i + 1, x)));
        }
    }
}
