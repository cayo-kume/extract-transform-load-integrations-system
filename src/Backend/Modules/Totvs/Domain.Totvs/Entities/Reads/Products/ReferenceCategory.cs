using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// categoria da referência, tabela totvs.TotvsSkusReferenceCategories
    /// </summary>
    public class ReferenceCategory
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;   // Category, SubCategory, Manufacturer, Filter

        public ReferenceCategory() { }

        public ReferenceCategory(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, Dtos.Reads.Products.ReferenceCategory dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 20);
        }
    }
}
