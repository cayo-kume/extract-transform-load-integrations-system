using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// bloqueio do produto por empresa, tabela totvs.TotvsProductsBranchesBlocked
    /// </summary>
    public class BranchProductBlocked
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int branchCode { get; private set; }
        public bool? isBlocked { get; private set; }

        public BranchProductBlocked() { }

        public BranchProductBlocked(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.BranchProductBlocked dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            isBlocked = NullableConverters.ToNullableBoolean(dto.isBlocked);
        }
    }
}
