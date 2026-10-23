using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// nota fiscal do pedido (code e serial nulos viram 0 e vazio, fazem parte da chave), tabela totvs.TotvsSaleOrdersInvoices
    /// </summary>
    public class Invoice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int code { get; private set; }
        public string serial { get; private set; } = String.Empty;
        public string accessKey { get; private set; } = String.Empty;
        public DateTime? issueDate { get; private set; }
        public string status { get; private set; } = String.Empty;
        public string shippingCompanyName { get; private set; } = String.Empty;
        public int? packageNumber { get; private set; }
        public decimal? grossWeight { get; private set; }
        public decimal? netWeight { get; private set; }
        public string trackingCode { get; private set; } = String.Empty;
        public decimal? discountPercentage { get; private set; }
        public decimal? quantity { get; private set; }
        public decimal? productValue { get; private set; }
        public decimal? additionalValue { get; private set; }
        public decimal? shippingValue { get; private set; }
        public decimal? insuranceValue { get; private set; }
        public decimal? ipiValue { get; private set; }
        public decimal? totalValue { get; private set; }
        public int? transactionBranchCode { get; private set; }
        public DateTime? transactionDate { get; private set; }
        public int? transactionCode { get; private set; }

        // electronic (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string electronicAccessKey { get; private set; } = String.Empty;
        public string electronicElectronicInvoiceStatus { get; private set; } = String.Empty;
        public long? electronicReceipt { get; private set; }
        public DateTime? electronicReceivementDate { get; private set; }

        public Invoice() { }

        public Invoice(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Invoice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            serial = ConverterValidatorsExtensions.StringLengthValidation(dto.serial, 3);
            accessKey = ConverterValidatorsExtensions.StringLengthValidation(dto.accessKey, 200);
            issueDate = NullableConverters.ToNullableDateTime(dto.issueDate);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 20);
            shippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompanyName, 60);
            packageNumber = NullableConverters.ToNullableInt32(dto.packageNumber);
            grossWeight = NullableConverters.ToNullableDecimal(dto.grossWeight);
            netWeight = NullableConverters.ToNullableDecimal(dto.netWeight);
            trackingCode = ConverterValidatorsExtensions.StringLengthValidation(dto.trackingCode, 40);
            discountPercentage = NullableConverters.ToNullableDecimal(dto.discountPercentage);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            productValue = NullableConverters.ToNullableDecimal(dto.productValue);
            additionalValue = NullableConverters.ToNullableDecimal(dto.additionalValue);
            shippingValue = NullableConverters.ToNullableDecimal(dto.shippingValue);
            insuranceValue = NullableConverters.ToNullableDecimal(dto.InsuranceValue);
            ipiValue = NullableConverters.ToNullableDecimal(dto.ipiValue);
            totalValue = NullableConverters.ToNullableDecimal(dto.totalValue);
            transactionBranchCode = NullableConverters.ToNullableInt32(dto.transactionBranchCode);
            transactionDate = NullableConverters.ToNullableDateTime(dto.transactionDate);
            transactionCode = NullableConverters.ToNullableInt32(dto.transactionCode);
            electronicAccessKey = ConverterValidatorsExtensions.StringLengthValidation(dto.electronic?.accessKey, 200);
            electronicElectronicInvoiceStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.electronic?.electronicInvoiceStatus, 20);
            electronicReceipt = (String.IsNullOrWhiteSpace(dto.electronic?.receipt) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.electronic?.receipt));
            electronicReceivementDate = NullableConverters.ToNullableDateTime(dto.electronic?.receivementDate);
        }
    }
}
