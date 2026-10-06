using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Classification
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int typeCode { get; private set; }
        public string typeName { get; private set; }
        public string typeNameAux { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }
        public string nameAux { get; private set; }

        public Classification() { }

        public Classification(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.Classification classificationDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(classificationDto.typeCode);
            typeName = classificationDto.typeName?.Length > 60 ? classificationDto.typeName.Substring(0, 60) : classificationDto.typeName;
            typeNameAux = classificationDto.typeNameAux?.Length > 60 ? classificationDto.typeNameAux.Substring(0, 60) : classificationDto.typeNameAux;
            code = classificationDto.code?.Length > 60 ? classificationDto.code.Substring(0, 60) : classificationDto.code;
            name = classificationDto.name?.Length > 60 ? classificationDto.name.Substring(0, 60) : classificationDto.name;
            nameAux = classificationDto.nameAux?.Length > 60 ? classificationDto.nameAux.Substring(0, 60) : classificationDto.nameAux;
        }
    }
}
