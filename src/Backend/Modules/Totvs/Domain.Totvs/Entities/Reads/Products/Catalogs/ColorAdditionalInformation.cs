using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// informação adicional da cor por empresa (AdditionalColorModel), tabela totvs.TotvsColorsAdditionalInformation
    /// </summary>
    public class ColorAdditionalInformation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string colorCode { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public string groupName { get; private set; } = String.Empty;
        public string auxiliaryCode { get; private set; } = String.Empty;
        public string auxiliaryName { get; private set; } = String.Empty;
        public string pantoneCode { get; private set; } = String.Empty;
        public string colorType { get; private set; } = String.Empty;
        public string colorTypeName { get; private set; } = String.Empty;
        public bool? isActive { get; private set; }
        public bool? isFinishedProduct { get; private set; }
        public bool? isRawMaterial { get; private set; }
        public bool? isBulkMaterial { get; private set; }

        public ColorAdditionalInformation() { }

        public ColorAdditionalInformation(string doc_company, DateTime integrated_or_updated_at, string colorCode, Dtos.Reads.Products.Catalogs.ColorAdditionalInformation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.colorCode = colorCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            groupName = ConverterValidatorsExtensions.StringLengthValidation(dto.groupName, 10);
            auxiliaryCode = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryCode, 10);
            auxiliaryName = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryName, 80);
            pantoneCode = ConverterValidatorsExtensions.StringLengthValidation(dto.pantoneCode, 40);
            colorType = ConverterValidatorsExtensions.StringLengthValidation(dto.colorType, 2);
            colorTypeName = ConverterValidatorsExtensions.StringLengthValidation(dto.colorTypeName, 40);
            isActive = NullableConverters.ToNullableBoolean(dto.isActive);
            isFinishedProduct = NullableConverters.ToNullableBoolean(dto.isFinishedProduct);
            isRawMaterial = NullableConverters.ToNullableBoolean(dto.isRawMaterial);
            isBulkMaterial = NullableConverters.ToNullableBoolean(dto.isBulkMaterial);
        }
    }
}
