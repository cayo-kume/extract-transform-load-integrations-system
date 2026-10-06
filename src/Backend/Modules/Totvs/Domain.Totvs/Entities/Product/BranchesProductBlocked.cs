using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class BranchesProductBlocked
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int branchCode { get; private set; }
        public bool isBlocked { get; private set; }

        public BranchesProductBlocked() { }

        public BranchesProductBlocked(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.BranchesProductBlocked branchesProductBlockedDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(branchesProductBlockedDto.branchCode);
            isBlocked = ConverterValidatorsExtensions.ConvertToBooleanValidation(branchesProductBlockedDto.isBlocked);
        }
    }
}
