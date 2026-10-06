using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class AdditionalField
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; }
        public string type { get; private set; }
        public string value { get; private set; }

        public AdditionalField() { }

        public AdditionalField(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.AdditionalField additionalFieldDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(additionalFieldDto.code);
            name = additionalFieldDto.name?.Length > 60 ? additionalFieldDto.name.Substring(0, 60) : additionalFieldDto.name;
            type = additionalFieldDto.type?.Length > 60 ? additionalFieldDto.type.Substring(0, 60) : additionalFieldDto.type;
            value = additionalFieldDto.value?.Length > 250 ? additionalFieldDto.value.Substring(0, 250) : additionalFieldDto.value;
        }
    }
}
