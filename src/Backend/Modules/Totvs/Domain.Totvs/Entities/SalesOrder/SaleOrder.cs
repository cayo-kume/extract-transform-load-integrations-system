using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.SalesOrder
{
    public class SaleOrder
    {
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int guideCode { get; private set; }
        public int sellerCode { get; private set; }
        public int customerCode { get; private set; }
        public int representativeCode { get; private set; }
        public int operationCode { get; private set; }
        public int paymentConditionCode { get; private set; }
        public int quantity { get; private set; }
        public int shippingCompanyCode { get; private set; }
        public int redispatchingShippingCompanyCode { get; private set; }
        public int reasonBlockingCode { get; private set; }
        public int reasonCancellationCode { get; private set; }
        public int omniSiteId { get; private set; }
        public int priceTableCode { get; private set; }
        public int rollMeasureCode { get; private set; }
        
        public decimal totalAmountOrder { get; private set; }
        public decimal netValue { get; private set; }
        public decimal freightValue { get; private set; }
        public decimal grossValue { get; private set; }
        public decimal discountValue { get; private set; }
        public decimal additionalValue { get; private set; }
        
        public bool hasPdvTransaction { get; private set; }
        public bool hasFinancialProcessed { get; private set; }
        
        public DateTime insertDate { get; private set; }
        public DateTime orderDate { get; private set; }
        public DateTime maxChangeFilterDate { get; private set; }
        public DateTime arrivalDate { get; private set; }
        public DateTime cancellationLimitDate { get; private set; }
        public DateTime billingForecastDate { get; private set; }
        public DateTime billingLimitDate { get; private set; }
        public DateTime lastBillingDate { get; private set; }
        public DateTime paymentBaseDate { get; private set; }

        public string orderId { get; private set; }
        public string customerOrderCode { get; private set; }
        public string integrationCode { get; private set; }
        public string guideCpfCnpj { get; private set; }
        public string sellerCpfCnpj { get; private set; }
        public string customerCpfCnpj { get; private set; }
        public string customerName { get; private set; }
        public string representativeCpfCnpj { get; private set; }
        public string representativeName { get; private set; }
        public string operationName { get; private set; }
        public string paymentConditionName { get; private set; }
        public string freightType { get; private set; }
        public string redispatchingFreightType { get; private set; }
        public string redispatchingShippingCompanyCpfCnpj { get; private set; }
        public string redispatchingShippingCompanyName { get; private set; }
        public string statusOrder { get; private set; }
        public string reasonBlockingDescription { get; private set; }
        public string reasonCancellationDescription { get; private set; }
        public string experienceType { get; private set; }
        public string batchOrder { get; private set; }
        public string rollMeasureDescription { get; private set; }
        public string ecommerceStage { get; private set; }

        public string shippingCompanyCpfCnpj { get; private set; }
        public string shippingCompanyName { get; private set; }
        public string shippingService { get; private set; }
        public string shippingServiceName { get; private set; }
        
        public int shippingNumber { get; private set; }
        public int shippingAddressSequence { get; private set; }
        public int shippingIbgeCityCode { get; private set; }
        public int shippingBcbCountryCode { get; private set; }
        public int shippingPostOfficeBox { get; private set; }

        public string shippingAddressType { get; private set; }
        public string shippingPublicPlace { get; private set; }
        public string shippingAddress { get; private set; }
        public string shippingComplement { get; private set; }
        public string shippingNeighborhood { get; private set; }
        public string shippingCityName { get; private set; }
        public string shippingStateAbbreviation { get; private set; }
        public string shippingCep { get; private set; }
        public string shippingCountryName { get; private set; }
        public string shippingReference { get; private set; }

        [SkipProperty]
        public List<Product> items { get; private set; } = new List<Product>();
        
        [SkipProperty]
        public List<Invoice> invoices { get; private set; } = new List<Invoice>();
        
        [SkipProperty]
        public List<Observation> observations { get; private set; } = new List<Observation>();
        
        [SkipProperty]
        public List<InvoiceObservation> invoiceObservations { get; private set; } = new List<InvoiceObservation>();
        
        [SkipProperty]
        public List<RepresentativeObservation> representativeObservations { get; private set; } = new List<RepresentativeObservation>();
        
        [SkipProperty]
        public List<Classification> classifications { get; private set; } = new List<Classification>();
        
        [SkipProperty]
        public List<Discount> discounts { get; private set; } = new List<Discount>();
        
        [SkipProperty]
        public List<Commissioned> commissioneds { get; private set; } = new List<Commissioned>();

        [SkipProperty]
        public List<Count> counts { get; private set; } = new List<Count>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        public SaleOrder() { }

        public SaleOrder(Domain.Totvs.Dtos.SalesOrder.Item saleOrderDto)
        {
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.orderCode);
            guideCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.guideCode);
            sellerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.sellerCode);
            customerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.customerCode);
            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.representativeCode);
            operationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.operationCode);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.paymentConditionCode);
            quantity = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.quantity);
            shippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingCompanyCode);
            redispatchingShippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.redispatchingShippingCompanyCode);
            reasonBlockingCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.reasonBlockingCode);
            reasonCancellationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.reasonCancellationCode);
            omniSiteId = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.omniSiteId);
            priceTableCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.priceTableCode);
            rollMeasureCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.rollMeasureCode);
            
            netValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.netValue);
            freightValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.freightValue);
            totalAmountOrder = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.totalAmountOrder);
            grossValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.grossValue);
            discountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.discountValue);
            additionalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(saleOrderDto.additionalValue);

            hasPdvTransaction = ConverterValidatorsExtensions.ConvertToBooleanValidation(saleOrderDto.hasPdvTransaction);
            hasFinancialProcessed = ConverterValidatorsExtensions.ConvertToBooleanValidation(saleOrderDto.hasFinancialProcessed);

            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.insertDate);
            orderDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.orderDate);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.maxChangeFilterDate);
            arrivalDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.arrivalDate);
            cancellationLimitDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.cancellationLimitDate);
            billingForecastDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.billingForecastDate);
            billingLimitDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.billingLimitDate);
            lastBillingDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.lastBillingDate);
            paymentBaseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(saleOrderDto.paymentBaseDate);

            orderId = saleOrderDto.orderId?.Length > 60 ? saleOrderDto.orderId?.Substring(0, 60) : saleOrderDto.orderId;
            customerOrderCode = saleOrderDto.customerOrderCode?.Length > 60 ? saleOrderDto.customerOrderCode?.Substring(0, 60) : saleOrderDto.customerOrderCode;
            integrationCode = saleOrderDto.integrationCode?.Length > 60 ? saleOrderDto.integrationCode?.Substring(0, 60) : saleOrderDto.integrationCode;
            guideCpfCnpj = saleOrderDto.guideCpfCnpj?.Length > 60 ? saleOrderDto.guideCpfCnpj?.Substring(0, 60) : saleOrderDto.guideCpfCnpj;
            sellerCpfCnpj = saleOrderDto.sellerCpfCnpj?.Length > 60 ? saleOrderDto.sellerCpfCnpj?.Substring(0, 60) : saleOrderDto.sellerCpfCnpj;
            customerCpfCnpj = saleOrderDto.customerCpfCnpj?.Length > 60 ? saleOrderDto.customerCpfCnpj?.Substring(0, 60) : saleOrderDto.customerCpfCnpj;
            customerName = saleOrderDto.customerName?.Length > 60 ? saleOrderDto.customerName?.Substring(0, 60) : saleOrderDto.customerName;
            representativeCpfCnpj = saleOrderDto.representativeCpfCnpj?.Length > 60 ? saleOrderDto.representativeCpfCnpj?.Substring(0, 60) : saleOrderDto.representativeCpfCnpj;
            representativeName = saleOrderDto.representativeName?.Length > 60 ? saleOrderDto.representativeName?.Substring(0, 60) : saleOrderDto.representativeName;
            operationName = saleOrderDto.operationName?.Length > 60 ? saleOrderDto.operationName?.Substring(0, 60) : saleOrderDto.operationName;
            paymentConditionName = saleOrderDto.paymentConditionName?.Length > 60 ? saleOrderDto.paymentConditionName?.Substring(0, 60) : saleOrderDto.paymentConditionName;
            freightType = saleOrderDto.freightType?.Length > 60 ? saleOrderDto.freightType?.Substring(0, 60) : saleOrderDto.freightType;
            redispatchingFreightType = saleOrderDto.redispatchingFreightType?.Length > 60 ? saleOrderDto.redispatchingFreightType?.Substring(0, 60) : saleOrderDto.redispatchingFreightType;
            redispatchingShippingCompanyCpfCnpj = saleOrderDto.redispatchingShippingCompanyCpfCnpj?.Length > 60 ? saleOrderDto.redispatchingShippingCompanyCpfCnpj?.Substring(0, 60) : saleOrderDto.redispatchingShippingCompanyCpfCnpj;
            redispatchingShippingCompanyName = saleOrderDto.redispatchingShippingCompanyName?.Length > 60 ? saleOrderDto.redispatchingShippingCompanyName?.Substring(0, 60) : saleOrderDto.redispatchingShippingCompanyName;
            statusOrder = saleOrderDto.statusOrder?.Length > 60 ? saleOrderDto.statusOrder?.Substring(0, 60) : saleOrderDto.statusOrder;
            reasonBlockingDescription = saleOrderDto.reasonBlockingDescription?.Length > 60 ? saleOrderDto.reasonBlockingDescription?.Substring(0, 60) : saleOrderDto.reasonBlockingDescription;
            reasonCancellationDescription = saleOrderDto.reasonCancellationDescription?.Length > 60 ? saleOrderDto.reasonCancellationDescription?.Substring(0, 60) : saleOrderDto.reasonCancellationDescription;
            experienceType = saleOrderDto.experienceType?.Length > 60 ? saleOrderDto.experienceType?.Substring(0, 60) : saleOrderDto.experienceType;
            batchOrder = saleOrderDto.batchOrder?.Length > 60 ? saleOrderDto.batchOrder?.Substring(0, 60) : saleOrderDto.batchOrder;
            rollMeasureDescription = saleOrderDto.rollMeasureDescription?.Length > 60 ? saleOrderDto.rollMeasureDescription?.Substring(0, 60) : saleOrderDto.rollMeasureDescription;
            ecommerceStage = saleOrderDto.ecommerceStage?.Length > 60 ? saleOrderDto.ecommerceStage?.Substring(0, 60) : saleOrderDto.ecommerceStage;

            shippingCompanyCpfCnpj = saleOrderDto.shippingCompanyCpfCnpj?.Length > 60 ? saleOrderDto.shippingCompanyCpfCnpj?.Substring(0, 60) : saleOrderDto.shippingCompanyCpfCnpj;
            shippingCompanyName = saleOrderDto.shippingCompanyName?.Length > 60 ? saleOrderDto.shippingCompanyName?.Substring(0, 60) : saleOrderDto.shippingCompanyName;
            shippingService = saleOrderDto.shippingService?.Length > 60 ? saleOrderDto.shippingService?.Substring(0, 60) : saleOrderDto.shippingService;
            shippingServiceName = saleOrderDto.shippingServiceName?.Length > 60 ? saleOrderDto.shippingServiceName?.Substring(0, 60) : saleOrderDto.shippingServiceName;

            shippingNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingAddress?.number);
            shippingAddressSequence = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingAddress?.addressSequence);
            shippingIbgeCityCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingAddress?.ibgeCityCode);
            shippingBcbCountryCode = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingAddress?.bcbCountryCode);
            shippingPostOfficeBox = ConverterValidatorsExtensions.ConvertToInt32Validation(saleOrderDto.shippingAddress?.postOfficeBox);

            shippingAddressType = saleOrderDto.shippingAddress?.addressType?.Length > 60 ? saleOrderDto.shippingAddress?.addressType?.Substring(0, 60) : saleOrderDto.shippingAddress?.addressType;
            shippingPublicPlace = saleOrderDto.shippingAddress?.publicPlace?.Length > 60 ? saleOrderDto.shippingAddress?.publicPlace?.Substring(0, 60) : saleOrderDto.shippingAddress?.publicPlace;
            shippingAddress = saleOrderDto.shippingAddress?.address?.Length > 60 ? saleOrderDto.shippingAddress?.address?.Substring(0, 60) : saleOrderDto.shippingAddress?.address;
            shippingComplement = saleOrderDto.shippingAddress?.complement?.Length > 60 ? saleOrderDto.shippingAddress?.complement?.Substring(0, 60) : saleOrderDto.shippingAddress?.complement;
            shippingNeighborhood = saleOrderDto.shippingAddress?.neighborhood?.Length > 60 ? saleOrderDto.shippingAddress?.neighborhood?.Substring(0, 60) : saleOrderDto.shippingAddress?.neighborhood;
            shippingCityName = saleOrderDto.shippingAddress?.cityName?.Length > 60 ? saleOrderDto.shippingAddress?.cityName?.Substring(0, 60) : saleOrderDto.shippingAddress?.cityName;
            shippingStateAbbreviation = saleOrderDto.shippingAddress?.stateAbbreviation?.Length > 60 ? saleOrderDto.shippingAddress?.stateAbbreviation?.Substring(0, 60) : saleOrderDto.shippingAddress?.stateAbbreviation;
            shippingCep = saleOrderDto.shippingAddress?.cep?.Length > 60 ? saleOrderDto.shippingAddress?.cep?.Substring(0, 60) : saleOrderDto.shippingAddress?.cep;
            shippingCountryName = saleOrderDto.shippingAddress?.countryName?.Length > 60 ? saleOrderDto.shippingAddress?.countryName?.Substring(0, 60) : saleOrderDto.shippingAddress?.countryName;
            shippingReference = saleOrderDto.shippingAddress?.reference?.Length > 60 ? saleOrderDto.shippingAddress?.reference?.Substring(0, 60) : saleOrderDto.shippingAddress?.reference;

            Responses.Add(saleOrderDto.orderCode, JsonConvert.SerializeObject(saleOrderDto));

            saleOrderDto.items?.ForEach(item =>
                items.Add(new Product(item, orderCode, branchCode)
            ));

            saleOrderDto.invoices?.ForEach(invoice =>
                invoices.Add(new Invoice(invoice, orderCode, branchCode)
            ));

            saleOrderDto.observations?.ForEach(observation =>
                observations.Add(new Observation(observation, orderCode, branchCode)
            ));

            saleOrderDto.invoiceObservations?.ForEach(invoiceObservation =>
                invoiceObservations.Add(new InvoiceObservation(invoiceObservation, orderCode, branchCode)
            ));

            saleOrderDto.representativeObservations?.ForEach(representativeObservation =>
                representativeObservations.Add(new RepresentativeObservation(representativeObservation, orderCode, branchCode)
            ));

            saleOrderDto.classifications?.ForEach(classification =>
                classifications.Add(new Classification(classification, orderCode, branchCode)
            ));

            saleOrderDto.discounts?.ForEach(discount =>
                discounts.Add(new Discount(discount, orderCode, branchCode)
            ));

            saleOrderDto.commissioneds?.ForEach(commissioned =>
                commissioneds.Add(new Commissioned(commissioned, orderCode, branchCode)
            ));

            saleOrderDto.counts?.ForEach(count =>
                counts.Add(new Count(count, orderCode)
            ));
        }
    }
}
