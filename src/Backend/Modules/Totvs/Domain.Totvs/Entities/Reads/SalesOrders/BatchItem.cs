using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// item de lote do produto do pedido (batchBranchCode é o branchCode do lote), tabela totvs.TotvsSaleOrdersProductsBatchItems
    /// </summary>
    public class BatchItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int productCode { get; private set; }
        public int sequence { get; private set; }
        public int batchBranchCode { get; private set; }
        public int code { get; private set; }
        public decimal? quantity { get; private set; }

        public BatchItem() { }

        public BatchItem(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, int productCode, Dtos.Reads.SalesOrders.BatchItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;
            this.productCode = productCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            batchBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
        }
    }
}
