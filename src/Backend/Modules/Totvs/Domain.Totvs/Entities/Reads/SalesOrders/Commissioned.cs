using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// comissionado do pedido, tabela totvs.TotvsSaleOrdersCommissioneds
    /// </summary>
    public class Commissioned
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int representativeCode { get; private set; }
        public string representativeName { get; private set; } = String.Empty;
        public decimal? commissionPercentageBilling { get; private set; }
        public decimal? commissionPercentageReceipt { get; private set; }
        public string commissionedType { get; private set; } = String.Empty;

        public Commissioned() { }

        public Commissioned(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Commissioned dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.representativeCode);
            representativeName = ConverterValidatorsExtensions.StringLengthValidation(dto.representativeName, 60);
            commissionPercentageBilling = NullableConverters.ToNullableDecimal(dto.commissionPercentageBilling);
            commissionPercentageReceipt = NullableConverters.ToNullableDecimal(dto.commissionPercentageReceipt);
            commissionedType = ConverterValidatorsExtensions.StringLengthValidation(dto.commissionedType, 30);
        }
    }
}
