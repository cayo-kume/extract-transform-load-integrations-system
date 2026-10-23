using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// detalhe de e-commerce do produto (descriptionType é a chave), tabela totvs.TotvsProductsWebDetails
    /// </summary>
    public class WebDetail
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int descriptionType { get; private set; }
        public string type { get; private set; } = String.Empty;
        public string title { get; private set; } = String.Empty;
        public string? description { get; private set; }   // nvarchar(max)

        public WebDetail() { }

        public WebDetail(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.WebDetail dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            descriptionType = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.descriptionType);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 60);
            title = ConverterValidatorsExtensions.StringLengthValidation(dto.title, 250);
            description = dto.description;
        }
    }
}
