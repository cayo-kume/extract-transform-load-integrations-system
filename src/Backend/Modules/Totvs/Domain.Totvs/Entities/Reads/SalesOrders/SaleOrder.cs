using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// pedido de venda (POST sales-order/v2/orders/search), tabela totvs.TotvsSaleOrders; tamanhos do swagger da API Sales Order
    /// </summary>
    public class SaleOrder
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public string orderId { get; private set; } = String.Empty;
        public string customerOrderCode { get; private set; } = String.Empty;
        public DateTime? insertDate { get; private set; }
        public string integrationCode { get; private set; } = String.Empty;
        public int? guideCode { get; private set; }
        public string guideCpfCnpj { get; private set; } = String.Empty;
        public int? sellerCode { get; private set; }
        public string sellerCpfCnpj { get; private set; } = String.Empty;
        public DateTime? orderDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }
        public DateTime? arrivalDate { get; private set; }
        public int customerCode { get; private set; }
        public string customerCpfCnpj { get; private set; } = String.Empty;
        public string customerName { get; private set; } = String.Empty;
        public int? representativeCode { get; private set; }
        public string representativeCpfCnpj { get; private set; } = String.Empty;
        public string representativeName { get; private set; } = String.Empty;
        public int operationCode { get; private set; }
        public string operationName { get; private set; } = String.Empty;
        public int paymentConditionCode { get; private set; }
        public string paymentConditionName { get; private set; } = String.Empty;
        public decimal? quantity { get; private set; }
        public decimal? grossValue { get; private set; }
        public decimal? discountValue { get; private set; }
        public decimal? netValue { get; private set; }
        public string freightType { get; private set; } = String.Empty;
        public decimal? freightValue { get; private set; }
        public int? shippingCompanyCode { get; private set; }
        public string shippingCompanyCpfCnpj { get; private set; } = String.Empty;
        public string shippingCompanyName { get; private set; } = String.Empty;
        public string redispatchingFreightType { get; private set; } = String.Empty;
        public int? redispatchingShippingCompanyCode { get; private set; }
        public string redispatchingShippingCompanyCpfCnpj { get; private set; } = String.Empty;
        public string redispatchingShippingCompanyName { get; private set; } = String.Empty;
        public decimal? totalAmountOrder { get; private set; }
        public string statusOrder { get; private set; } = String.Empty;
        public int? reasonBlockingCode { get; private set; }
        public string reasonBlockingDescription { get; private set; } = String.Empty;
        public int? reasonCancellationCode { get; private set; }
        public string reasonCancellationDescription { get; private set; } = String.Empty;
        public int? omniSiteId { get; private set; }
        public decimal? additionalValue { get; private set; }
        public string experienceType { get; private set; } = String.Empty;
        public bool? hasPdvTransaction { get; private set; }
        public bool? hasFinancialProcessed { get; private set; }
        public int? priceTableCode { get; private set; }
        public string shippingService { get; private set; } = String.Empty;
        public string shippingServiceName { get; private set; } = String.Empty;
        public DateTime? cancellationLimitDate { get; private set; }
        public DateTime? billingForecastDate { get; private set; }
        public DateTime? billingLimitDate { get; private set; }
        public DateTime? lastBillingDate { get; private set; }
        public DateTime? paymentBaseDate { get; private set; }
        public string batchOrder { get; private set; } = String.Empty;
        public int? rollMeasureCode { get; private set; }
        public string rollMeasureDescription { get; private set; } = String.Empty;
        public string ecommerceStage { get; private set; } = String.Empty;

        // shippingAddress (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string shippingAddressAddressType { get; private set; } = String.Empty;
        public string shippingAddressPublicPlace { get; private set; } = String.Empty;
        public string shippingAddressAddress { get; private set; } = String.Empty;
        public int? shippingAddressNumber { get; private set; }
        public string shippingAddressComplement { get; private set; } = String.Empty;
        public int? shippingAddressAddressSequence { get; private set; }
        public string shippingAddressNeighborhood { get; private set; } = String.Empty;
        public int? shippingAddressIbgeCityCode { get; private set; }
        public string shippingAddressCityName { get; private set; } = String.Empty;
        public string shippingAddressStateAbbreviation { get; private set; } = String.Empty;
        public string shippingAddressCep { get; private set; } = String.Empty;
        public int? shippingAddressBcbCountryCode { get; private set; }
        public string shippingAddressCountryName { get; private set; } = String.Empty;
        public int? shippingAddressPostOfficeBox { get; private set; }
        public string shippingAddressReference { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Product> items { get; private set; } = [];

        [SkipProperty]
        public List<Invoice> invoices { get; private set; } = [];

        [SkipProperty]
        public List<Observation> observations { get; private set; } = [];

        [SkipProperty]
        public List<InvoiceObservation> invoiceObservations { get; private set; } = [];

        [SkipProperty]
        public List<RepresentativeObservation> representativeObservations { get; private set; } = [];

        [SkipProperty]
        public List<Classification> classifications { get; private set; } = [];

        [SkipProperty]
        public List<Discount> discounts { get; private set; } = [];

        [SkipProperty]
        public List<Commissioned> commissioneds { get; private set; } = [];

        [SkipProperty]
        public List<Count> counts { get; private set; } = [];

        public SaleOrder() { }

        public SaleOrder(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesOrders.SaleOrder dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.orderCode);
            orderId = ConverterValidatorsExtensions.StringLengthValidation(dto.orderId, 100);
            customerOrderCode = ConverterValidatorsExtensions.StringLengthValidation(dto.customerOrderCode, 100);
            insertDate = NullableConverters.ToNullableDateTime(dto.insertDate);
            integrationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.integrationCode, 100);
            guideCode = NullableConverters.ToNullableInt32(dto.guideCode);
            guideCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.guideCpfCnpj, 14);
            sellerCode = NullableConverters.ToNullableInt32(dto.sellerCode);
            sellerCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.sellerCpfCnpj, 14);
            orderDate = NullableConverters.ToNullableDateTime(dto.orderDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            arrivalDate = NullableConverters.ToNullableDateTime(dto.arrivalDate);
            customerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.customerCode);
            customerCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.customerCpfCnpj, 14);
            customerName = ConverterValidatorsExtensions.StringLengthValidation(dto.customerName, 60);
            representativeCode = NullableConverters.ToNullableInt32(dto.representativeCode);
            representativeCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.representativeCpfCnpj, 14);
            representativeName = ConverterValidatorsExtensions.StringLengthValidation(dto.representativeName, 60);
            operationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.operationCode);
            operationName = ConverterValidatorsExtensions.StringLengthValidation(dto.operationName, 80);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.paymentConditionCode);
            paymentConditionName = ConverterValidatorsExtensions.StringLengthValidation(dto.paymentConditionName, 40);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            grossValue = NullableConverters.ToNullableDecimal(dto.grossValue);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
            netValue = NullableConverters.ToNullableDecimal(dto.netValue);
            freightType = ConverterValidatorsExtensions.StringLengthValidation(dto.freightType, 30);
            freightValue = NullableConverters.ToNullableDecimal(dto.freightValue);
            shippingCompanyCode = NullableConverters.ToNullableInt32(dto.shippingCompanyCode);
            shippingCompanyCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompanyCpfCnpj, 14);
            shippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompanyName, 60);
            redispatchingFreightType = ConverterValidatorsExtensions.StringLengthValidation(dto.redispatchingFreightType, 30);
            redispatchingShippingCompanyCode = NullableConverters.ToNullableInt32(dto.redispatchingShippingCompanyCode);
            redispatchingShippingCompanyCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.redispatchingShippingCompanyCpfCnpj, 14);
            redispatchingShippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.redispatchingShippingCompanyName, 60);
            totalAmountOrder = NullableConverters.ToNullableDecimal(dto.totalAmountOrder);
            statusOrder = ConverterValidatorsExtensions.StringLengthValidation(dto.statusOrder, 30);
            reasonBlockingCode = NullableConverters.ToNullableInt32(dto.reasonBlockingCode);
            reasonBlockingDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.reasonBlockingDescription, 60);
            reasonCancellationCode = NullableConverters.ToNullableInt32(dto.reasonCancellationCode);
            reasonCancellationDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.reasonCancellationDescription, 60);
            omniSiteId = NullableConverters.ToNullableInt32(dto.omniSiteId);
            additionalValue = NullableConverters.ToNullableDecimal(dto.additionalValue);
            experienceType = ConverterValidatorsExtensions.StringLengthValidation(dto.experienceType, 30);
            hasPdvTransaction = NullableConverters.ToNullableBoolean(dto.hasPdvTransaction);
            hasFinancialProcessed = NullableConverters.ToNullableBoolean(dto.hasFinancialProcessed);
            priceTableCode = NullableConverters.ToNullableInt32(dto.priceTableCode);
            shippingService = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingService, 10);
            shippingServiceName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingServiceName, 200);
            cancellationLimitDate = NullableConverters.ToNullableDateTime(dto.cancellationLimitDate);
            billingForecastDate = NullableConverters.ToNullableDateTime(dto.billingForecastDate);
            billingLimitDate = NullableConverters.ToNullableDateTime(dto.billingLimitDate);
            lastBillingDate = NullableConverters.ToNullableDateTime(dto.lastBillingDate);
            paymentBaseDate = NullableConverters.ToNullableDateTime(dto.paymentBaseDate);
            batchOrder = ConverterValidatorsExtensions.StringLengthValidation(dto.batchOrder, 10);
            rollMeasureCode = NullableConverters.ToNullableInt32(dto.rollMeasureCode);
            rollMeasureDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.rollMeasureDescription, 80);
            ecommerceStage = ConverterValidatorsExtensions.StringLengthValidation(dto.ecommerceStage, 40);

            shippingAddressAddressType = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.addressType, 20);
            shippingAddressPublicPlace = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.publicPlace, 10);
            shippingAddressAddress = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.address, 60);
            shippingAddressNumber = NullableConverters.ToNullableInt32(dto.shippingAddress?.number);
            shippingAddressComplement = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.complement, 65);
            shippingAddressAddressSequence = NullableConverters.ToNullableInt32(dto.shippingAddress?.addressSequence);
            shippingAddressNeighborhood = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.neighborhood, 65);
            shippingAddressIbgeCityCode = NullableConverters.ToNullableInt32(dto.shippingAddress?.ibgeCityCode);
            shippingAddressCityName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.cityName, 60);
            shippingAddressStateAbbreviation = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.stateAbbreviation, 3);
            shippingAddressCep = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.cep, 8);
            shippingAddressBcbCountryCode = NullableConverters.ToNullableInt32(dto.shippingAddress?.bcbCountryCode);
            shippingAddressCountryName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.countryName, 60);
            shippingAddressPostOfficeBox = NullableConverters.ToNullableInt32(dto.shippingAddress?.postOfficeBox);
            shippingAddressReference = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingAddress?.reference, 60);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real)
            items.AddRange((dto.items ?? []).Select(x => new Product(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => x.productCode).Select(g => g.Last()));
            invoices.AddRange((dto.invoices ?? []).Select(x => new Invoice(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => (x.code, x.serial.ToUpperInvariant())).Select(g => g.Last()));
            observations.AddRange((dto.observations ?? []).Select((x, i) => new Observation(doc_company, integrated_or_updated_at, branchCode, orderCode, i + 1, x)));
            invoiceObservations.AddRange((dto.invoiceObservations ?? []).Select((x, i) => new InvoiceObservation(doc_company, integrated_or_updated_at, branchCode, orderCode, i + 1, x)));
            representativeObservations.AddRange((dto.representativeObservations ?? []).Select((x, i) => new RepresentativeObservation(doc_company, integrated_or_updated_at, branchCode, orderCode, i + 1, x)));
            classifications.AddRange((dto.classifications ?? []).Select(x => new Classification(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => (x.typeCode, x.code.ToUpperInvariant())).Select(g => g.Last()));
            discounts.AddRange((dto.discounts ?? []).Select(x => new Discount(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => x.typeDiscountCode).Select(g => g.Last()));
            commissioneds.AddRange((dto.commissioneds ?? []).Select(x => new Commissioned(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => x.representativeCode).Select(g => g.Last()));
            counts.AddRange((dto.counts ?? []).Select(x => new Count(doc_company, integrated_or_updated_at, branchCode, orderCode, x)).GroupBy(x => (x.countBranchCode, x.code)).Select(g => g.Last()));
        }
    }
}
