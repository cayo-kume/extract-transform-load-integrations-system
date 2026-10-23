using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// código de grupo de integração da referência, tabela totvs.TotvsSkusIntegrationGroupCodes
    /// </summary>
    public class IntegrationGroupCode
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int code { get; private set; }

        public IntegrationGroupCode() { }

        public IntegrationGroupCode(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, Dtos.Reads.Products.IntegrationGroupCode dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
        }
    }
}
