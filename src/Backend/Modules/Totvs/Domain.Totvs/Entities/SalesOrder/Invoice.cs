using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class Invoice
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }
        
        public int code { get; private set; }
        public int packageNumber { get; private set; }

        public decimal grossWeight { get; private set; }
        public decimal netWeight { get; private set; }
        public decimal discountPercentage { get; private set; }

        public int quantity { get; private set; }

        public decimal productValue { get; private set; }
        public decimal additionalValue { get; private set; }
        public decimal shippingValue { get; private set; }
        public decimal insuranceValue { get; private set; }
        public decimal ipiValue { get; private set; }
        public decimal totalValue { get; private set; }

        public int transactionBranchCode { get; private set; }
        public int transactionCode { get; private set; }

        public DateTime issueDate { get; private set; }
        public DateTime transactionDate { get; private set; }
        
        public string accessKey { get; private set; }
        public string serial { get; private set; }
        public string status { get; private set; }
        public string shippingCompanyName { get; private set; }
        public string trackingCode { get; private set; }

        public int electronicReceipt { get; private set; }
        public DateTime electronicReceivementDate { get; private set; }
        public string electronicAccessKey { get; private set; }
        public string electronicInvoiceStatus { get; private set; }

        public Invoice() { }

        public Invoice(Domain.Totvs.Dtos.SalesOrder.Invoice invoiceDto, int saleOrderCode, int branchCode)
        {
            this.saleOrderCode = saleOrderCode;
            this.branchCode = branchCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.code);
            packageNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.packageNumber);
            grossWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.grossWeight);
            netWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.netWeight);
            discountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.discountPercentage);
            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.quantity);
            transactionBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.transactionBranchCode);
            transactionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.transactionCode);
            
            productValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.productValue);
            additionalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.additionalValue);
            shippingValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.shippingValue);
            insuranceValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.InsuranceValue);
            ipiValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.ipiValue);
            totalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.totalValue);

            issueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.issueDate);
            transactionDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.transactionDate);

            accessKey = invoiceDto.accessKey?.Length > 60 ? invoiceDto.accessKey?.Substring(0, 60) : invoiceDto.accessKey;
            serial = invoiceDto.serial?.Length > 60 ? invoiceDto.serial?.Substring(0, 60) : invoiceDto.serial;
            status = invoiceDto.status?.Length > 60 ? invoiceDto.status?.Substring(0, 60) : invoiceDto.status;
            shippingCompanyName = invoiceDto.shippingCompanyName?.Length > 60 ? invoiceDto.shippingCompanyName?.Substring(0, 60) : invoiceDto.shippingCompanyName;
            trackingCode = invoiceDto.trackingCode?.Length > 60 ? invoiceDto.trackingCode?.Substring(0, 60) : invoiceDto.trackingCode;

            electronicReceipt = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.electronic?.receipt);
            electronicReceivementDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.electronic?.receivementDate);
            electronicAccessKey = invoiceDto.electronic?.accessKey?.Length > 60 ? invoiceDto.electronic?.accessKey?.Substring(0, 60) : invoiceDto.electronic?.accessKey;
            electronicInvoiceStatus = invoiceDto.electronic?.electronicInvoiceStatus?.Length > 60 ? invoiceDto.electronic?.electronicInvoiceStatus?.Substring(0, 60) : invoiceDto.electronic?.electronicInvoiceStatus;
        }
    }
}
