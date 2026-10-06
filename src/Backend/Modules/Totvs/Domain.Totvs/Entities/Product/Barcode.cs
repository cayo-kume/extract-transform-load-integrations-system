using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Barcode
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public string code { get; private set; }
        public decimal quantity { get; private set; }
        public bool isMainCode { get; private set; }

        public Barcode() { }

        public Barcode(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.Barcode barcodeDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = barcodeDto.code?.Length > 60 ? barcodeDto.code.Substring(0, 60) : barcodeDto.code;
            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(barcodeDto.quantity);
            isMainCode = ConverterValidatorsExtensions.ConvertToBooleanValidation(barcodeDto.isMainCode);
        }
    }
}
