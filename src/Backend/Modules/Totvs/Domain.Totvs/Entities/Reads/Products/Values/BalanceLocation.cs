using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Values
{
    /// <summary>
    /// local do produto na loja (locations do balances/search), tabela totvs.TotvsBalancesLocations
    /// </summary>
    public class BalanceLocation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int productCode { get; private set; }
        public string locationCode { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;

        public BalanceLocation() { }

        public BalanceLocation(string doc_company, DateTime integrated_or_updated_at, int productCode, Dtos.Reads.Products.Values.ProductLocation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            this.productCode = productCode;
            locationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.locationCode, 20);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
        }
    }
}
