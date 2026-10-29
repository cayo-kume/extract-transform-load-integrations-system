using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// atributo (metadado) do produto, tabela linxcommerce.LinxCommerceProductsMetadataValues
    /// </summary>
    public class ProductMetadataValue
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public int PropertyMetadataID { get; private set; }
        public string PropertyName { get; private set; } = String.Empty;
        public string DisplayName { get; private set; } = String.Empty;
        public string PropertyGroup { get; private set; } = String.Empty;
        public string FormattedValue { get; private set; } = String.Empty;

        public ProductMetadataValue() { }

        public ProductMetadataValue(string doc_company, DateTime integrated_or_updated_at, int ProductID, Dtos.Reads.Catalog.ProductMetadataValue dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ProductID = ProductID;

            PropertyMetadataID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.PropertyMetadataID);
            PropertyName = ConverterValidatorsExtensions.StringLengthValidation(dto.PropertyName, 250);
            DisplayName = ConverterValidatorsExtensions.StringLengthValidation(dto.DisplayName, 250);
            PropertyGroup = ConverterValidatorsExtensions.StringLengthValidation(dto.PropertyGroup, 250);
            FormattedValue = dto.FormattedValue ?? String.Empty;
        }
    }
}
