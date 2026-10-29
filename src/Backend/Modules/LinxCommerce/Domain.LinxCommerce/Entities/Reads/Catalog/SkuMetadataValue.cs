using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// atributo (metadado) do SKU, tabela linxcommerce.LinxCommerceSkusMetadataValues
    /// </summary>
    public class SkuMetadataValue
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public int PropertyMetadataID { get; private set; }
        public string PropertyName { get; private set; } = String.Empty;
        public string DisplayName { get; private set; } = String.Empty;
        public string PropertyGroup { get; private set; } = String.Empty;
        public int? InputType { get; private set; }
        public string IntegrationID { get; private set; } = String.Empty;
        public string Value { get; private set; } = String.Empty;
        public string FormattedValue { get; private set; } = String.Empty;
        public string SerializedValue { get; private set; } = String.Empty;
        public string SerializedBlobValue { get; private set; } = String.Empty;

        public SkuMetadataValue() { }

        public SkuMetadataValue(string doc_company, DateTime integrated_or_updated_at, int ProductID, Dtos.Reads.Catalog.SkuMetadataValue dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ProductID = ProductID;

            PropertyMetadataID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.PropertyMetadataID);
            PropertyName = ConverterValidatorsExtensions.StringLengthValidation(dto.PropertyName, 250);
            DisplayName = ConverterValidatorsExtensions.StringLengthValidation(dto.DisplayName, 250);
            PropertyGroup = ConverterValidatorsExtensions.StringLengthValidation(dto.PropertyGroup, 250);
            InputType = NullableConverters.ToNullableInt32(dto.InputType);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
            Value = dto.Value ?? String.Empty;
            FormattedValue = dto.FormattedValue ?? String.Empty;
            SerializedValue = dto.SerializedValue ?? String.Empty;
            SerializedBlobValue = dto.SerializedBlobValue ?? String.Empty;
        }
    }
}
