namespace Domain.Totvs.Entities.SalesOrder
{
    public class InvoiceObservation
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }

        public string observation { get; private set; }

        public InvoiceObservation() { }

        public InvoiceObservation(Domain.Totvs.Dtos.SalesOrder.InvoiceObservation invoiceObservationDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

             observation = invoiceObservationDto.observation?.Length > 60 ? invoiceObservationDto.observation?.Substring(0, 60) : invoiceObservationDto.observation;
        }
    }
}
