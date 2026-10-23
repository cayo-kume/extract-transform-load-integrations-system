using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// informação do vendedor por empresa, tabela totvs.TotvsSellersBranchInformations
    /// </summary>
    public class BranchInformation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int sellerCode { get; private set; }
        public int branchCode { get; private set; }
        public int? auxiliaryCode { get; private set; }
        public bool? isInactive { get; private set; }
        public int? sellerTypeCode { get; private set; }
        public string sellerTypeDescription { get; private set; } = String.Empty;

        public BranchInformation() { }

        public BranchInformation(string doc_company, DateTime integrated_or_updated_at, int sellerCode, Dtos.Reads.Sellers.BranchInformation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.sellerCode = sellerCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            auxiliaryCode = NullableConverters.ToNullableInt32(dto.auxiliaryCode);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);
            sellerTypeCode = NullableConverters.ToNullableInt32(dto.sellerTypeCode);
            sellerTypeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.sellerTypeDescription, 60);
        }
    }
}
