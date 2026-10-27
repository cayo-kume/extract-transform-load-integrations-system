using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// classificação de pedido de venda da tabela de preço (SalesOrderClassificationModel), tabela totvs.TotvsPriceTablesSalesOrderClassifications
    /// </summary>
    public class PriceTableSalesOrderClassification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public PriceTableSalesOrderClassification() { }

        public PriceTableSalesOrderClassification(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, Dtos.Reads.Products.Catalogs.PriceTableSalesOrderClassification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 40);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
        }
    }
}
