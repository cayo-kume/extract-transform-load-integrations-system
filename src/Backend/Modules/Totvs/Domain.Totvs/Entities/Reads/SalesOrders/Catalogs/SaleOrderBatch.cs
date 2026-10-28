using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// lote de pedidos de venda do PEDFM018 (GET sales-order/v2/batch-items, BatchOrderModel; o código vai em batchOrder da capa do pedido; status 1 aberto, 2 fechado), tabela totvs.TotvsSaleOrderBatches
    /// </summary>
    public class SaleOrderBatch
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string code { get; private set; } = String.Empty;
        public string description { get; private set; } = String.Empty;
        public int? status { get; private set; }
        public DateTime? startDate { get; private set; }
        public DateTime? endDate { get; private set; }
        public DateTime? closingDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public SaleOrderBatch() { }

        public SaleOrderBatch(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesOrders.Catalogs.SaleOrderBatch dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            status = NullableConverters.ToNullableInt32(dto.status);
            startDate = NullableConverters.ToNullableDateTime(dto.startDate);
            endDate = NullableConverters.ToNullableDateTime(dto.endDate);
            closingDate = NullableConverters.ToNullableDateTime(dto.closingDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
