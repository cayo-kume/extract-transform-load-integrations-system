using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// contagem (conferência) do pedido (countBranchCode é o branchCode da contagem), tabela totvs.TotvsSaleOrdersCounts
    /// </summary>
    public class Count
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int countBranchCode { get; private set; }
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string statusCount { get; private set; } = String.Empty;
        public int? packagingTypeCode { get; private set; }
        public string packagingTypeDescription { get; private set; } = String.Empty;
        public int? separator { get; private set; }
        public string separatorName { get; private set; } = String.Empty;

        public Count() { }

        public Count(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Count dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            countBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            statusCount = ConverterValidatorsExtensions.StringLengthValidation(dto.statusCount, 20);
            packagingTypeCode = NullableConverters.ToNullableInt32(dto.packagingTypeCode);
            packagingTypeDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.packagingTypeDescription, 60);
            separator = NullableConverters.ToNullableInt32(dto.separator);
            separatorName = ConverterValidatorsExtensions.StringLengthValidation(dto.separatorName, 60);
        }
    }
}
