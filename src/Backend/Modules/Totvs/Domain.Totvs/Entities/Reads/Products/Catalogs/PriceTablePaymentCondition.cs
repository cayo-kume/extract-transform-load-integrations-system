using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// condição de pagamento da tabela de preço (PaymentConditionModel), tabela totvs.TotvsPriceTablesPaymentConditions
    /// </summary>
    public class PriceTablePaymentCondition
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int priceTableCode { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;

        public PriceTablePaymentCondition() { }

        public PriceTablePaymentCondition(string doc_company, DateTime integrated_or_updated_at, int priceTableCode, Dtos.Reads.Products.Catalogs.PriceTablePaymentCondition dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.priceTableCode = priceTableCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
        }
    }
}
