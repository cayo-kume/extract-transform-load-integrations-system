using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.PurchaseOrders
{
    /// <summary>
    /// pedido de compra (POST purchase-order/v2/search), tabela totvs.TotvsPurchaseOrders; o swagger da API Purchase Order não traz tamanhos: os do código antigo, conferidos numa amostra real
    /// </summary>
    public class PurchaseOrder
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public string supplierOrderCode { get; private set; } = String.Empty;
        public int? supplierCode { get; private set; }
        public string supplierCpfCnpj { get; private set; } = String.Empty;
        public string supplierName { get; private set; } = String.Empty;
        public int? buyerCode { get; private set; }
        public string buyerCpfCnpj { get; private set; } = String.Empty;
        public string buyerName { get; private set; } = String.Empty;
        public int operationCode { get; private set; }
        public string operationName { get; private set; } = String.Empty;
        public string shippingCompanyName { get; private set; } = String.Empty;
        public int paymentConditionCode { get; private set; }
        public string paymentConditionName { get; private set; } = String.Empty;
        public string paymentType { get; private set; } = String.Empty;
        public string status { get; private set; } = String.Empty;
        public string freightType { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }
        public DateTime? registrationDate { get; private set; }
        public DateTime? deliveryForecastDate { get; private set; }
        public DateTime? deliveryDeadlineDate { get; private set; }
        public DateTime? basePaymentDate { get; private set; }
        public decimal? productValue { get; private set; }
        public decimal? quantity { get; private set; }
        public decimal? ipiValue { get; private set; }
        public decimal? totalAmountOrder { get; private set; }

        [SkipProperty]
        public List<Product> items { get; private set; } = [];

        [SkipProperty]
        public List<Request> requests { get; private set; } = [];

        public PurchaseOrder() { }

        public PurchaseOrder(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.PurchaseOrders.PurchaseOrder dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.orderCode);
            supplierOrderCode = ConverterValidatorsExtensions.StringLengthValidation(dto.supplierOrderCode, 60);
            supplierCode = NullableConverters.ToNullableInt32(dto.supplierCode);
            supplierCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.supplierCpfCnpj, 14);
            supplierName = ConverterValidatorsExtensions.StringLengthValidation(dto.supplierName, 100);
            buyerCode = NullableConverters.ToNullableInt32(dto.buyerCode);
            buyerCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.buyerCpfCnpj, 14);
            buyerName = ConverterValidatorsExtensions.StringLengthValidation(dto.buyerName, 100);
            operationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.operationCode);
            operationName = ConverterValidatorsExtensions.StringLengthValidation(dto.operationName, 100);
            shippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompanyName, 100);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.paymentConditionCode);
            paymentConditionName = ConverterValidatorsExtensions.StringLengthValidation(dto.paymentConditionName, 100);
            paymentType = ConverterValidatorsExtensions.StringLengthValidation(dto.paymentType, 30);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 30);
            freightType = ConverterValidatorsExtensions.StringLengthValidation(dto.freightType, 30);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            registrationDate = NullableConverters.ToNullableDateTime(dto.registrationDate);
            deliveryForecastDate = NullableConverters.ToNullableDateTime(dto.deliveryForecastDate);
            deliveryDeadlineDate = NullableConverters.ToNullableDateTime(dto.deliveryDeadlineDate);
            basePaymentDate = NullableConverters.ToNullableDateTime(dto.basePaymentDate);
            productValue = NullableConverters.ToNullableDecimal(dto.productValue);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            ipiValue = NullableConverters.ToNullableDecimal(dto.ipiValue);
            totalAmountOrder = NullableConverters.ToNullableDecimal(dto.totalAmountOrder);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real)
            items.AddRange((dto.items ?? []).Select(x => new Product(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => x.productCode).Select(g => g.Last()));
            requests.AddRange((dto.requests ?? []).Select(x => new Request(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => (x.requestBranchCode, x.requestCode)).Select(g => g.Last()));
        }
    }
}
