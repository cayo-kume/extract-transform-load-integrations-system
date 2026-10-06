using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Classification
    {
        public int saleOrderCode { get; private set; }
        public int branchCode { get; private set; }

        public int typeCode { get; private set; }

        public string typeName { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }

        public Classification() { }

        public Classification(Domain.Totvs.Dtos.SalesOrder.Classification classificationDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(classificationDto.typeCode);

            typeName = classificationDto.typeName?.Length > 60 ? classificationDto.typeName?.Substring(0, 60) : classificationDto.typeName;
            code = classificationDto.code?.Length > 60 ? classificationDto.code?.Substring(0, 60) : classificationDto.code;
            name = classificationDto.name?.Length > 60 ? classificationDto.name?.Substring(0, 60) : classificationDto.name;
            this.branchCode = branchCode;
        }
    }
}
