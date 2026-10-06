using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Seller
{
    public class BranchInformation
    {
        public int sellerCode { get; private set; }

        public int branchCode { get; private set; }
        public int auxiliaryCode { get; private set; }
        public int sellerTypeCode { get; private set; }

        public bool isInactive { get; private set; }

        public string sellerTypeDescription { get; private set; }

        public BranchInformation() { }

        public BranchInformation(Domain.Totvs.Dtos.Seller.BranchInformation branchInformationDto, int sellerCode)
        {
            this.sellerCode = sellerCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(branchInformationDto.branchCode);
            auxiliaryCode = ConverterValidatorsExtensions.ConvertToInt32Validation(branchInformationDto.auxiliaryCode);
            sellerTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(branchInformationDto.sellerTypeCode);

            isInactive = ConverterValidatorsExtensions.ConvertToBooleanValidation(branchInformationDto.isInactive);

            sellerTypeDescription = branchInformationDto.sellerTypeDescription?.Length > 60 ? branchInformationDto.sellerTypeDescription?.Substring(0, 60) : branchInformationDto.sellerTypeDescription;
        }
    }
}
