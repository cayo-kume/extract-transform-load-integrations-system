using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Fiscal;

namespace Domain.Totvs.Entities.Fiscal
{
    public class Product
    {
        public int invoiceSequence { get; private set; }
        public int invoiceItemsSequence { get; private set; }

        public int productCode { get; private set; }
        public int dealerCode { get; private set; }

        public string productName { get; private set; }

        public decimal quantity { get; private set; }
        public decimal unitGrossValue { get; private set; }
        public decimal unitDiscountValue { get; private set; }
        public decimal unitNetValue { get; private set; }
        public decimal grossValue { get; private set; }
        public decimal discountValue { get; private set; }
        public decimal netValue { get; private set; }

        [SkipProperty]
        public List<ReferencedInvoice> referencedInvoices { get; private set; } = new List<ReferencedInvoice>();

        public Product() { }

        public Product(Domain.Totvs.Dtos.Fiscal.Product productDto, int invoiceSequence, int invoiceItemsSequence)
        {
            this.invoiceSequence = invoiceSequence;
            this.invoiceItemsSequence = invoiceItemsSequence;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.productCode);
            dealerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.dealerCode);

            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.quantity);
            unitGrossValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.unitGrossValue);
            unitDiscountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.unitDiscountValue);
            unitNetValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.unitNetValue);
            grossValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.grossValue);
            discountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.discountValue);
            netValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(productDto.netValue);

            productName = productDto.productName?.Length > 250 ? productDto.productName?.Substring(0, 250) : productDto.productName;

            productDto.referencedInvoices?.ForEach(referencedInvoice =>
                referencedInvoices.Add(new ReferencedInvoice(referencedInvoice, invoiceSequence, invoiceItemsSequence, productCode)
            ));
        }
    }
}
