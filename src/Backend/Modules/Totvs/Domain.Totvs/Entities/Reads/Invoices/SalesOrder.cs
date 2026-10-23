using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// pedido de venda da nota (orderBranchCode é o branchCode do pedido; vínculo sem loja ou número é descartado), tabela totvs.TotvsInvoicesSalesOrders
    /// </summary>
    public class SalesOrder
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int orderBranchCode { get; private set; }
        public int orderCode { get; private set; }
        public string orderId { get; private set; } = String.Empty;
        public string customerOrderCode { get; private set; } = String.Empty;

        public SalesOrder() { }

        public SalesOrder(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, Dtos.Reads.Invoices.SalesOrder dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;

            orderBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.orderCode);
            orderId = ConverterValidatorsExtensions.StringLengthValidation(dto.orderId, 100);
            customerOrderCode = ConverterValidatorsExtensions.StringLengthValidation(dto.customerOrderCode, 100);
        }
    }
}
