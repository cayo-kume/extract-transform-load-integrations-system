using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// nota fiscal (POST fiscal/v2/invoices/search), tabela totvs.TotvsInvoices. Chave: doc_company + branchCode + invoiceSequence + invoiceDate (conferida no
    /// swagger: é como a própria API identifica uma nota, src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.1)
    /// </summary>
    public class Invoice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public string branchCnpj { get; private set; } = String.Empty;
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int? personCode { get; private set; }
        public string personName { get; private set; } = String.Empty;
        public int? invoiceCode { get; private set; }
        public string serialCode { get; private set; } = String.Empty;
        public string invoiceStatus { get; private set; } = String.Empty;
        public int? transactionBranchCode { get; private set; }
        public DateTime? transactionDate { get; private set; }
        public int? transactionCode { get; private set; }
        public string inclusionComponentCode { get; private set; } = String.Empty;
        public string peripheralPdvCode { get; private set; } = String.Empty;
        public string versionPdv { get; private set; } = String.Empty;
        public string mobileVersion { get; private set; } = String.Empty;
        public string idDocumentPDV { get; private set; } = String.Empty;
        public int? userCode { get; private set; }
        public string origin { get; private set; } = String.Empty;
        public int? documentType { get; private set; }
        public string operationType { get; private set; } = String.Empty;
        public int? operationCode { get; private set; }
        public string operatioName { get; private set; } = String.Empty;
        public DateTime? issueDate { get; private set; }
        public DateTime? releaseDate { get; private set; }
        public string exitTime { get; private set; } = String.Empty;
        public DateTime? lastchangeDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }
        public int? paymentConditionCode { get; private set; }
        public string paymentConditionName { get; private set; } = String.Empty;
        public decimal? discountPercentage { get; private set; }
        public decimal? quantity { get; private set; }
        public decimal? productValue { get; private set; }
        public decimal? additionalValue { get; private set; }
        public decimal? shippingValue { get; private set; }
        public decimal? insuranceValue { get; private set; }
        public decimal? ipiValue { get; private set; }
        public decimal? baseIcmsValue { get; private set; }
        public decimal? icmsValue { get; private set; }
        public decimal? icmsSubStValue { get; private set; }
        public decimal? totalValue { get; private set; }
        public int? terminalCode { get; private set; }
        public string sellerCpf { get; private set; } = String.Empty;
        public string? observationNFE { get; private set; }

        // eletronic (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string eletronicAccessKey { get; private set; } = String.Empty;
        public string eletronicElectronicInvoiceStatus { get; private set; } = String.Empty;
        public long? eletronicReceipt { get; private set; }
        public DateTime? eletronicReceivementDate { get; private set; }
        public long? eletronicDisableProtocol { get; private set; }
        public DateTime? eletronicDisableDate { get; private set; }
        public string eletronicNfseCode { get; private set; } = String.Empty;

        // ecf (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? ecfCoupon { get; private set; }
        public string ecfSerialMachine { get; private set; } = String.Empty;

        // sat (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? satSatCode { get; private set; }
        public int? satDvNumber { get; private set; }
        public string satCnfNumber { get; private set; } = String.Empty;
        public string satCancellationKey { get; private set; } = String.Empty;
        public int? satSatSequence { get; private set; }
        public string satSerialMachine { get; private set; } = String.Empty;

        // shippingCompany (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? shippingCompanyShippingCompanyCode { get; private set; }
        public string shippingCompanyShippingCompanyName { get; private set; } = String.Empty;
        public string shippingCompanyFreitghtType { get; private set; } = String.Empty;
        public string shippingCompanyFreitghtTypeRedispatch { get; private set; } = String.Empty;
        public int? shippingCompanyRedispatchCarrierCode { get; private set; }
        public string shippingCompanyRedispatchCarrierName { get; private set; } = String.Empty;
        public int? shippingCompanyCityCodeRedispatch { get; private set; }
        public string shippingCompanyCityNameRedispatch { get; private set; } = String.Empty;
        public string shippingCompanyCityUfRedispatch { get; private set; } = String.Empty;
        public decimal? shippingCompanyFreightValue { get; private set; }
        public string shippingCompanyShippingCompanyType { get; private set; } = String.Empty;
        public string shippingCompanyPublicPlaceTypeName { get; private set; } = String.Empty;
        public string shippingCompanyPublicPlaceName { get; private set; } = String.Empty;
        public int? shippingCompanyPublicPlaceCode { get; private set; }
        public string shippingCompanyNeighborhood { get; private set; } = String.Empty;
        public string shippingCompanyCityName { get; private set; } = String.Empty;
        public string shippingCompanyStateAbbreviation { get; private set; } = String.Empty;
        public string shippingCompanyCep { get; private set; } = String.Empty;
        public int? shippingCompanyPostOfficeBox { get; private set; }
        public string shippingCompanyNumberStateRegistration { get; private set; } = String.Empty;
        public string shippingCompanyCpfCnpj { get; private set; } = String.Empty;
        public string shippingCompanyPlaqueCode { get; private set; } = String.Empty;
        public string shippingCompanyPlaqueUf { get; private set; } = String.Empty;
        public int? shippingCompanyPackageNumber { get; private set; }
        public decimal? shippingCompanyGrossWeight { get; private set; }
        public decimal? shippingCompanyNetWeight { get; private set; }
        public string shippingCompanySpecies { get; private set; } = String.Empty;
        public string shippingCompanyBrandName { get; private set; } = String.Empty;
        public string shippingCompanyTrackingCode { get; private set; } = String.Empty;

        // person (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? personPersonCode { get; private set; }
        public string personPersonName { get; private set; } = String.Empty;
        public string personPersonType { get; private set; } = String.Empty;
        public int? personPostOfficeBox { get; private set; }
        public int? personAddressNumber { get; private set; }
        public string personCep { get; private set; } = String.Empty;
        public string personAddress { get; private set; } = String.Empty;
        public string personPublicPlace { get; private set; } = String.Empty;
        public string personComplement { get; private set; } = String.Empty;
        public string personNeighborhood { get; private set; } = String.Empty;
        public string personCity { get; private set; } = String.Empty;
        public string personStateAbbreviation { get; private set; } = String.Empty;
        public string personFoneNumber { get; private set; } = String.Empty;
        public string personRgIe { get; private set; } = String.Empty;
        public string personPersonCpfCnpj { get; private set; } = String.Empty;
        public int? personCityCode { get; private set; }

        // productionOrder (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? productionOrderBranchCode { get; private set; }
        public int? productionOrderCycleCode { get; private set; }
        public int? productionOrderOrderCode { get; private set; }
        public string productionOrderOrderRelationshipType { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Item> items { get; private set; } = [];

        [SkipProperty]
        public List<Payment> payments { get; private set; } = [];

        [SkipProperty]
        public List<SalesOrder> salesOrder { get; private set; } = [];

        [SkipProperty]
        public List<ObservationNF> observationNF { get; private set; } = [];

        [SkipProperty]
        public List<ReferencedTaxInvoice> referencedTaxInvoice { get; private set; } = [];

        public Invoice() { }

        public Invoice(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Invoices.Invoice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            branchCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.branchCnpj, 14);
            invoiceSequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.invoiceSequence);
            invoiceDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(dto.invoiceDate);
            personCode = NullableConverters.ToNullableInt32(dto.personCode);
            personName = ConverterValidatorsExtensions.StringLengthValidation(dto.personName, 100);
            invoiceCode = NullableConverters.ToNullableInt32(dto.invoiceCode);
            serialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.serialCode, 3);
            invoiceStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.invoiceStatus, 20);
            transactionBranchCode = NullableConverters.ToNullableInt32(dto.transactionBranchCode);
            transactionDate = NullableConverters.ToNullableDateTime(dto.transactionDate);
            transactionCode = NullableConverters.ToNullableInt32(dto.transactionCode);
            inclusionComponentCode = ConverterValidatorsExtensions.StringLengthValidation(dto.inclusionComponentCode, 20);
            peripheralPdvCode = ConverterValidatorsExtensions.StringLengthValidation(dto.peripheralPdvCode, 60);
            versionPdv = ConverterValidatorsExtensions.StringLengthValidation(dto.versionPdv, 60);
            mobileVersion = ConverterValidatorsExtensions.StringLengthValidation(dto.mobileVersion, 60);
            idDocumentPDV = ConverterValidatorsExtensions.StringLengthValidation(dto.idDocumentPDV, 100);
            userCode = NullableConverters.ToNullableInt32(dto.userCode);
            origin = ConverterValidatorsExtensions.StringLengthValidation(dto.origin, 20);
            documentType = NullableConverters.ToNullableInt32(dto.documentType);
            operationType = ConverterValidatorsExtensions.StringLengthValidation(dto.operationType, 20);
            operationCode = NullableConverters.ToNullableInt32(dto.operationCode);
            operatioName = ConverterValidatorsExtensions.StringLengthValidation(dto.operatioName, 100);
            issueDate = NullableConverters.ToNullableDateTime(dto.issueDate);
            releaseDate = NullableConverters.ToNullableDateTime(dto.releaseDate);
            exitTime = ConverterValidatorsExtensions.StringLengthValidation(dto.exitTime, 10);
            lastchangeDate = NullableConverters.ToNullableDateTime(dto.lastchangeDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            paymentConditionCode = NullableConverters.ToNullableInt32(dto.paymentConditionCode);
            paymentConditionName = ConverterValidatorsExtensions.StringLengthValidation(dto.paymentConditionName, 60);
            discountPercentage = NullableConverters.ToNullableDecimal(dto.discountPercentage);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            productValue = NullableConverters.ToNullableDecimal(dto.productValue);
            additionalValue = NullableConverters.ToNullableDecimal(dto.additionalValue);
            shippingValue = NullableConverters.ToNullableDecimal(dto.shippingValue);
            insuranceValue = NullableConverters.ToNullableDecimal(dto.insuranceValue);
            ipiValue = NullableConverters.ToNullableDecimal(dto.ipiValue);
            baseIcmsValue = NullableConverters.ToNullableDecimal(dto.baseIcmsValue);
            icmsValue = NullableConverters.ToNullableDecimal(dto.icmsValue);
            icmsSubStValue = NullableConverters.ToNullableDecimal(dto.icmsSubStValue);
            totalValue = NullableConverters.ToNullableDecimal(dto.totalValue);
            terminalCode = NullableConverters.ToNullableInt32(dto.terminalCode);
            sellerCpf = ConverterValidatorsExtensions.StringLengthValidation(dto.sellerCpf, 14);
            observationNFE = dto.observationNFE;

            eletronicAccessKey = ConverterValidatorsExtensions.StringLengthValidation(dto.eletronic?.accessKey, 50);
            eletronicElectronicInvoiceStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.eletronic?.electronicInvoiceStatus, 20);
            eletronicReceipt = (String.IsNullOrWhiteSpace(dto.eletronic?.receipt) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.eletronic?.receipt));
            eletronicReceivementDate = NullableConverters.ToNullableDateTime(dto.eletronic?.receivementDate);
            eletronicDisableProtocol = (String.IsNullOrWhiteSpace(dto.eletronic?.disableProtocol) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.eletronic?.disableProtocol));
            eletronicDisableDate = NullableConverters.ToNullableDateTime(dto.eletronic?.disableDate);
            eletronicNfseCode = ConverterValidatorsExtensions.StringLengthValidation(dto.eletronic?.nfseCode, 60);

            ecfCoupon = NullableConverters.ToNullableInt32(dto.ecf?.coupon);
            ecfSerialMachine = ConverterValidatorsExtensions.StringLengthValidation(dto.ecf?.serialMachine, 60);

            satSatCode = NullableConverters.ToNullableInt32(dto.sat?.satCode);
            satDvNumber = NullableConverters.ToNullableInt32(dto.sat?.dvNumber);
            satCnfNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.sat?.cnfNumber, 20);
            satCancellationKey = ConverterValidatorsExtensions.StringLengthValidation(dto.sat?.cancellationKey, 60);
            satSatSequence = NullableConverters.ToNullableInt32(dto.sat?.satSequence);
            satSerialMachine = ConverterValidatorsExtensions.StringLengthValidation(dto.sat?.serialMachine, 60);

            shippingCompanyShippingCompanyCode = NullableConverters.ToNullableInt32(dto.shippingCompany?.shippingCompanyCode);
            shippingCompanyShippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.shippingCompanyName, 100);
            shippingCompanyFreitghtType = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.freitghtType, 30);
            shippingCompanyFreitghtTypeRedispatch = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.freitghtTypeRedispatch, 30);
            shippingCompanyRedispatchCarrierCode = NullableConverters.ToNullableInt32(dto.shippingCompany?.redispatchCarrierCode);
            shippingCompanyRedispatchCarrierName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.redispatchCarrierName, 100);
            shippingCompanyCityCodeRedispatch = NullableConverters.ToNullableInt32(dto.shippingCompany?.cityCodeRedispatch);
            shippingCompanyCityNameRedispatch = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.cityNameRedispatch, 60);
            shippingCompanyCityUfRedispatch = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.cityUfRedispatch, 2);
            shippingCompanyFreightValue = NullableConverters.ToNullableDecimal(dto.shippingCompany?.freightValue);
            shippingCompanyShippingCompanyType = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.shippingCompanyType, 30);
            shippingCompanyPublicPlaceTypeName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.publicPlaceTypeName, 10);
            shippingCompanyPublicPlaceName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.publicPlaceName, 100);
            shippingCompanyPublicPlaceCode = NullableConverters.ToNullableInt32(dto.shippingCompany?.publicPlaceCode);
            shippingCompanyNeighborhood = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.neighborhood, 60);
            shippingCompanyCityName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.cityName, 60);
            shippingCompanyStateAbbreviation = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.stateAbbreviation, 2);
            shippingCompanyCep = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.cep, 8);
            shippingCompanyPostOfficeBox = NullableConverters.ToNullableInt32(dto.shippingCompany?.postOfficeBox);
            shippingCompanyNumberStateRegistration = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.numberStateRegistration, 20);
            shippingCompanyCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.cpfCnpj, 14);
            shippingCompanyPlaqueCode = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.plaqueCode, 10);
            shippingCompanyPlaqueUf = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.plaqueUf, 2);
            shippingCompanyPackageNumber = NullableConverters.ToNullableInt32(dto.shippingCompany?.packageNumber);
            shippingCompanyGrossWeight = NullableConverters.ToNullableDecimal(dto.shippingCompany?.grossWeight);
            shippingCompanyNetWeight = NullableConverters.ToNullableDecimal(dto.shippingCompany?.netWeight);
            shippingCompanySpecies = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.species, 60);
            shippingCompanyBrandName = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.brandName, 60);
            shippingCompanyTrackingCode = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.trackingCode, 60);

            personPersonCode = NullableConverters.ToNullableInt32(dto.person?.personCode);
            personPersonName = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.personName, 100);
            personPersonType = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.personType, 5);
            personPostOfficeBox = NullableConverters.ToNullableInt32(dto.person?.postOfficeBox);
            personAddressNumber = NullableConverters.ToNullableInt32(dto.person?.addressNumber);
            personCep = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.cep, 8);
            personAddress = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.address, 100);
            personPublicPlace = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.publicPlace, 10);
            personComplement = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.complement, 100);
            personNeighborhood = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.neighborhood, 60);
            personCity = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.city, 60);
            personStateAbbreviation = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.stateAbbreviation, 2);
            personFoneNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.foneNumber, 20);
            personRgIe = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.rgIe, 20);
            personPersonCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.person?.personCpfCnpj, 14);
            personCityCode = NullableConverters.ToNullableInt32(dto.person?.cityCode);

            productionOrderBranchCode = NonZero(NullableConverters.ToNullableInt32(dto.productionOrder?.branchCode));
            productionOrderCycleCode = NonZero(NullableConverters.ToNullableInt32(dto.productionOrder?.cycleCode));
            productionOrderOrderCode = NonZero(NullableConverters.ToNullableInt32(dto.productionOrder?.orderCode));
            productionOrderOrderRelationshipType = ConverterValidatorsExtensions.StringLengthValidation(dto.productionOrder?.orderRelationshipType, 10);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real)
            items.AddRange((dto.items ?? []).Select(x => new Item(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            payments.AddRange((dto.payments ?? []).Select(x => new Payment(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, x)).GroupBy(x => (x.documentNumber, x.installment)).Select(g => g.Last()));
            salesOrder.AddRange((dto.salesOrder ?? []).Select(x => new SalesOrder(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, x)).Where(x => x.orderBranchCode != 0 && x.orderCode != 0).GroupBy(x => (x.orderBranchCode, x.orderCode)).Select(g => g.Last()));
            observationNF.AddRange((dto.observationNF ?? []).Select(x => new ObservationNF(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            referencedTaxInvoice.AddRange((dto.referencedTaxInvoice ?? []).Select(x => new ReferencedTaxInvoice(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, x)).GroupBy(x => (x.referencedInvoiceBranchCode, x.invoiceCode, x.invoiceSerialCode.ToUpperInvariant())).Select(g => g.Last()));
        }

        // productionOrder vem com zeros quando a nota não tem ordem de produção (testado)
        private static int? NonZero(int? value) => value == 0 ? null : value;
    }
}
