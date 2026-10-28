using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// classificação de pedido de venda e o seu tipo (GET sales-order/v2/classifications, ClassificationTypeRelationshipModel; type vem como número, 1 capa e 2 item, testado), tabela totvs.TotvsSaleClassifications
    /// </summary>
    public class SaleClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int typeCode { get; private set; }
        public string typeDescription { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public SaleClassification() { }

        public SaleClassification(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesOrders.Catalogs.SaleClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.typeDescription, 40);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 20);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
