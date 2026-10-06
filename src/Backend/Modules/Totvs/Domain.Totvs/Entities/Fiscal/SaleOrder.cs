using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class SaleOrder
    {
        public int invoiceSequence { get; private set; }
        
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        
        public string orderId { get; private set; }
        public string customerOrderCode { get; private set; }

        public SaleOrder() { }

        public SaleOrder(Domain.Totvs.Dtos.Fiscal.SalesOrder salesOrderDto, int invoiceSequence)
        {
            this.invoiceSequence = invoiceSequence;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(salesOrderDto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(salesOrderDto.orderCode);

            orderId = salesOrderDto.orderId?.Length > 60 ? salesOrderDto.orderId?.Substring(0, 60) : salesOrderDto.orderId;
            customerOrderCode = salesOrderDto.customerOrderCode?.Length > 60 ? salesOrderDto.customerOrderCode?.Substring(0, 60) : salesOrderDto.customerOrderCode;
        }
    }
}
