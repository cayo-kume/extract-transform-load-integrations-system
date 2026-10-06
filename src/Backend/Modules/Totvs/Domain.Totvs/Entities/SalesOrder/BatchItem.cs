using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class BatchItem
    {
        public int saleOrderCode { get; private set; }
        public int productCode { get; private set; }

        public int sequence { get; private set; }
        public int branchCode { get; private set; }
        public int code { get; private set; }
        public int quantity { get; private set; }

        public BatchItem() { }

        public BatchItem(Domain.Totvs.Dtos.SalesOrder.BatchItem batchItemDto, int productCode, int saleOrderCode)
        {
            this.saleOrderCode = saleOrderCode;
            this.productCode = productCode;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(batchItemDto.sequence);
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(batchItemDto.branchCode);
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(batchItemDto.code);
            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(batchItemDto.quantity);
        }
    }
}
