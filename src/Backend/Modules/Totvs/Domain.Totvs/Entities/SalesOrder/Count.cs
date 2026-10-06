using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Count
    {
        public int saleOrderCode { get; private set; }
        
        public int branchCode { get; private set; }
        public int code { get; private set; }
        public int packagingTypeCode { get; private set; }
        public int separator { get; private set; }
        
        public string description { get; private set; }
        public string statusCount { get; private set; }
        public string packagingTypeDescription { get; private set; }
        public string separatorName { get; private set; }

        public Count() { }

        public Count(Domain.Totvs.Dtos.SalesOrder.Count countDto, int saleOrderCode)
        {
            this.saleOrderCode = saleOrderCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(countDto.branchCode);
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(countDto.code);
            packagingTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(countDto.packagingTypeCode);
            separator = ConverterValidatorsExtensions.ConvertToInt32Validation(countDto.separator);

            description = countDto.description?.Length > 60 ? countDto.description?.Substring(0, 60) : countDto.description;
            statusCount = countDto.statusCount?.Length > 60 ? countDto.statusCount?.Substring(0, 60) : countDto.statusCount;
            packagingTypeDescription = countDto.packagingTypeDescription?.Length > 60 ? countDto.packagingTypeDescription?.Substring(0, 60) : countDto.packagingTypeDescription;
            separatorName = countDto.separatorName?.Length > 60 ? countDto.separatorName?.Substring(0, 60) : countDto.separatorName;
        }
    }
}
