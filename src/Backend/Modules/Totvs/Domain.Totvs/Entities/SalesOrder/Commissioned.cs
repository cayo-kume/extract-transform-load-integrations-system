using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Commissioned
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }
        
        public int representativeCode { get; private set; }

        public decimal commissionPercentageBilling { get; private set; }
        public decimal commissionPercentageReceipt { get; private set; }
        
        public string representativeName { get; private set; }
        public string commissionedType { get; private set; }

        public Commissioned() { }

        public Commissioned(Domain.Totvs.Dtos.SalesOrder.Commissioned commissionedDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(commissionedDto.representativeCode);
            commissionPercentageBilling = ConverterValidatorsExtensions.ConvertToDecimalValidation(commissionedDto.commissionPercentageBilling);
            commissionPercentageReceipt = ConverterValidatorsExtensions.ConvertToDecimalValidation(commissionedDto.commissionPercentageReceipt);

            representativeName = commissionedDto.representativeName?.Length > 60 ? commissionedDto.representativeName?.Substring(0, 60) : commissionedDto.representativeName;
            commissionedType = commissionedDto.commissionedType?.Length > 60 ? commissionedDto.commissionedType?.Substring(0, 60) : commissionedDto.commissionedType;
            this.branchCode = branchCode;
        }
    }
}
