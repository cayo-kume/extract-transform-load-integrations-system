namespace Domain.Totvs.Entities.Fiscal
{
    public class BarCode
    {
        public int invoiceSequence { get; private set; }
        public int invoiceItemsSequence { get; private set; }
        public string barcode { get; private set; }

        public BarCode() { }

        public BarCode(Domain.Totvs.Dtos.Fiscal.Barcode barcodeDto, int invoiceSequence, int invoiceItemsSequence)
        {
            this.invoiceSequence = invoiceSequence;
            this.invoiceItemsSequence = invoiceItemsSequence;
            barcode = barcodeDto.barcode?.Length > 60 ? barcodeDto.barcode?.Substring(0, 60) : barcodeDto.barcode;
        }
    }
}
