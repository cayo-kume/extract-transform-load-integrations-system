using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// campo adicional do produto, tabela totvs.TotvsProductsAdditionalFields
    /// </summary>
    public class AdditionalField
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public string? value { get; private set; }   // value: nvarchar(max), até 491 caracteres no teste

        public AdditionalField() { }

        public AdditionalField(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.AdditionalField dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 20);
            value = dto.value;
        }
    }
}
