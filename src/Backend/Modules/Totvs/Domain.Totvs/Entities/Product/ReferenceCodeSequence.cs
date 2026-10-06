using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class ReferenceCodeSequence
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }
        public int parentCode { get; private set; }

        public ReferenceCodeSequence() { }

        public ReferenceCodeSequence(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.ReferenceCodeSequence referenceCodeSequenceDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = referenceCodeSequenceDto.code?.Length > 60 ? referenceCodeSequenceDto.code.Substring(0, 60) : referenceCodeSequenceDto.code;
            name = referenceCodeSequenceDto.name?.Length > 250 ? referenceCodeSequenceDto.name.Substring(0, 250) : referenceCodeSequenceDto.name;
            parentCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referenceCodeSequenceDto.parentCode);
        }
    }
}
