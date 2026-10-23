using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.PurchaseOrders
{
    /// <summary>
    /// solicitação de compra ligada ao pedido (requestBranchCode é o branchCode da solicitação), tabela totvs.TotvsPurchaseOrdersRequests
    /// </summary>
    public class Request
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int requestBranchCode { get; private set; }
        public int requestCode { get; private set; }
        public DateTime? requestDate { get; private set; }

        public Request() { }

        public Request(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.PurchaseOrders.Request dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            requestBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            requestCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.requestCode);
            requestDate = NullableConverters.ToNullableDateTime(dto.requestDate);
        }
    }
}
