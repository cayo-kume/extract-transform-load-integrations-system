using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Detail
    {
        public int referenceId { get; private set; }
        public int typeCode { get; private set; }
        public string type { get; private set; }
        public string auxiliaryType { get; private set; }
        public string title { get; private set; }
        public string description { get; private set; }

        public Detail() { }

        public Detail(int referenceId, Dtos.Product.Detail detailDto)
        {
            this.referenceId = referenceId;
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(detailDto.typeCode);
            type = detailDto.type?.Length > 60 ? detailDto.type.Substring(0, 60) : detailDto.type;
            auxiliaryType = detailDto.auxiliaryType?.Length > 60 ? detailDto.auxiliaryType.Substring(0, 60) : detailDto.auxiliaryType;
            title = detailDto.title?.Length > 250 ? detailDto.title.Substring(0, 250) : detailDto.title;
            description = detailDto.description?.Length > 300 ? detailDto.description.Substring(0, 300) : detailDto.description;
        }
    }
}
