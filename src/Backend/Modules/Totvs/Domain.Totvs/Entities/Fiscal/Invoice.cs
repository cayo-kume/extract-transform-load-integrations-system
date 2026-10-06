using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.Fiscal
{
    public class Invoice
    {
        //invoice
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public int personCode { get; private set; }
        public int invoiceCode { get; private set; }
        public int transactionBranchCode { get; private set; }
        public int transactionCode { get; private set; }
        public int userCode { get; private set; }
        public int documentType { get; private set; }
        public int operationCode { get; private set; }
        public int paymentConditionCode { get; private set; }
        public int terminalCode { get; private set; }

        public DateTime invoiceDate { get; private set; }
        public DateTime transactionDate { get; private set; }
        public DateTime issueDate { get; private set; }
        public DateTime releaseDate { get; private set; }
        public DateTime lastchangeDate { get; private set; }
        public DateTime maxChangeFilterDate { get; private set; }
        public DateTime receivementDate { get; private set; }
        public DateTime disableDate { get; private set; }
        
        public decimal discountPercentage { get; private set; }
        public decimal quantity { get; private set; }
        public decimal productValue { get; private set; }
        public decimal additionalValue { get; private set; }
        public decimal shippingValue { get; private set; }
        public decimal insuranceValue { get; private set; }
        public decimal ipiValue { get; private set; }
        public decimal baseIcmsValue { get; private set; }
        public decimal icmsValue { get; private set; }
        public decimal icmsSubStValue { get; private set; }
        public decimal totalValue { get; private set; }

        public string branchCnpj { get; private set; }
        public string personName { get; private set; }
        public string serialCode { get; private set; }
        public string invoiceStatus { get; private set; }
        public string inclusionComponentCode { get; private set; }
        public string peripheralPdvCode { get; private set; }
        public string versionPdv { get; private set; }
        public string mobileVersion { get; private set; }
        public string idDocumentPDV { get; private set; }
        public string origin { get; private set; }
        public string operationType { get; private set; }
        public string operatioName { get; private set; }
        public string exitTime { get; private set; }
        public string paymentConditionName { get; private set; }
        public string sellerCpf { get; private set; }
        public string observationNFe { get; private set; }

        //eletronic
        public int receipt { get; private set; }
        public int disableProtocol { get; private set; }

        public string accessKey { get; private set; }
        public string electronicInvoiceStatus { get; private set; }

        //ecf
        public int ecfCoupon { get; private set; }

        public string ecfSerialMachine { get; private set; }

        //sat
        public int satCode { get; private set; }
        public int satDvNumber { get; private set; }
        public int satSequence { get; private set; }

        public string satCnfNumber { get; private set; }
        public string satCancellationKey { get; private set; }
        public string satSerialMachine { get; private set; }

        //shipping company
        public int shippingCompanyCode { get; private set; }
        public int shippingCompanyRedispatchCarrierCode { get; private set; }
        public int shippingCompanyCityCodeRedispatch { get; private set; }
        public int shippingCompanyPublicPlaceCode { get; private set; }
        public int shippingCompanyPackageNumber { get; private set; }

        public decimal shippingCompanyFreightValue { get; private set; }
        public decimal shippingCompanyGrossWeight { get; private set; }
        public decimal shippingCompanyNetWeight { get; private set; }

        public string shippingCompanyName { get; private set; }
        public string shippingCompanyFreitghtType { get; private set; }
        public string shippingCompanyFreitghtTypeRedispatch { get; private set; }
        public string shippingCompanyRedispatchCarrierName { get; private set; }
        public string shippingCompanyCityNameRedispatch { get; private set; }
        public string shippingCompanyCityUfRedispatch { get; private set; }
        public string shippingCompanyType { get; private set; }
        public string shippingCompanyPublicPlaceTypeName { get; private set; }
        public string shippingCompanyPublicPlaceName { get; private set; }
        public string shippingCompanyStateAbbreviation { get; private set; }
        public string shippingCompanyCep { get; private set; }
        public string shippingCompanyPostOfficeBox { get; private set; }
        public string shippingCompanyNumberStateRegistration { get; private set; }
        public string shippingCompanyCpfCnpj { get; private set; }
        public string shippingCompanyPlaqueCode { get; private set; }
        public string shippingCompanyPlaqueUf { get; private set; }
        public string shippingCompanySpecies { get; private set; }
        public string shippingCompanyBrandName { get; private set; }
        public string shippingCompanyTrackingCode { get; private set; }
        public string shippingCompanyNeighborhood { get; private set; }
        public string shippingCompanyCityName { get; private set; }

        //person
        public int personType { get; private set; }
        public int personPostOfficeBox { get; private set; }
        public int personAddressNumber { get; private set; }
        public int personCityCode { get; private set; }

        public string personCep { get; private set; }
        public string personAddress { get; private set; }
        public string personPublicPlace { get; private set; }
        public string personComplement { get; private set; }
        public string personNeighborhood { get; private set; }
        public string personCity { get; private set; }
        public string personStateAbbreviation { get; private set; }
        public string personFoneNumber { get; private set; }
        public string personRgIe { get; private set; }
        public string personCpfCnpj { get; private set; }
        public string productionOrderRelationshipType { get; private set; }

        //production order
        public int productionOrderBranchCode { get; private set; }
        public int productionOrderCycleCode { get; private set; }
        public int productionOrderCode { get; private set; }

        [SkipProperty]
        public List<SaleOrder> salesOrder { get; private set; } = new List<SaleOrder>();

        [SkipProperty]
        public List<ProductItem> items { get; private set; } = new List<ProductItem>();
        
        [SkipProperty]
        public List<Payment> payments { get; private set; } = new List<Payment>();
        
        [SkipProperty]
        public List<ObservationNF> observationNF { get; private set; } = new List<ObservationNF>();

        [SkipProperty]
        public List<ReferencedTax> referencedTaxInvoice { get; private set; } = new List<ReferencedTax>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        public Invoice() { }
        
        public Invoice(Domain.Totvs.Dtos.Fiscal.Item invoiceDto)
        {
            //invoice
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.branchCode);
            invoiceSequence = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.invoiceSequence);
            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.personCode);
            invoiceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.invoiceCode);
            transactionBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.transactionBranchCode);
            transactionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.transactionCode);
            userCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.userCode);
            documentType = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.documentType);
            operationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.operationCode);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.paymentConditionCode);
            terminalCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.terminalCode);

            invoiceDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.invoiceDate);
            transactionDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.transactionDate);
            issueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.issueDate);
            releaseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.releaseDate);
            lastchangeDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.lastchangeDate);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.maxChangeFilterDate);

            discountPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.discountPercentage);
            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.quantity);
            productValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.productValue);
            additionalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.additionalValue);
            shippingValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.shippingValue);
            insuranceValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.insuranceValue);
            ipiValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.ipiValue);
            baseIcmsValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.baseIcmsValue);
            icmsValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.icmsValue);
            icmsSubStValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.icmsSubStValue);
            totalValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.totalValue);

            branchCnpj = invoiceDto.branchCnpj?.Length > 14 ? invoiceDto.branchCnpj?.Substring(0, 14) : invoiceDto.branchCnpj;
            personName = invoiceDto.personName?.Length > 250 ? invoiceDto.personName?.Substring(0, 250) : invoiceDto.personName;
            serialCode = invoiceDto.serialCode?.Length > 60 ? invoiceDto.serialCode?.Substring(0, 60) : invoiceDto.serialCode;
            invoiceStatus = invoiceDto.invoiceStatus?.Length > 60 ? invoiceDto.invoiceStatus?.Substring(0, 60) : invoiceDto.invoiceStatus;
            inclusionComponentCode = invoiceDto.inclusionComponentCode?.Length > 60 ? invoiceDto.inclusionComponentCode?.Substring(0, 60) : invoiceDto.inclusionComponentCode;
            peripheralPdvCode = invoiceDto.peripheralPdvCode?.Length > 60 ? invoiceDto.peripheralPdvCode?.Substring(0, 60) : invoiceDto.peripheralPdvCode;
            versionPdv = invoiceDto.versionPdv?.Length > 60 ? invoiceDto.versionPdv?.Substring(0, 60) : invoiceDto.versionPdv;
            mobileVersion = invoiceDto.mobileVersion?.Length > 60 ? invoiceDto.mobileVersion?.Substring(0, 60) : invoiceDto.mobileVersion;
            idDocumentPDV = invoiceDto.idDocumentPDV?.Length > 60 ? invoiceDto.idDocumentPDV?.Substring(0, 60) : invoiceDto.idDocumentPDV;
            origin = invoiceDto.origin?.Length > 60 ? invoiceDto.origin?.Substring(0, 60) : invoiceDto.origin;
            operationType = invoiceDto.operationType?.Length > 60 ? invoiceDto.operationType?.Substring(0, 60) : invoiceDto.operationType;
            operatioName = invoiceDto.operatioName?.Length > 60 ? invoiceDto.operatioName?.Substring(0, 60) : invoiceDto.operatioName;
            exitTime = invoiceDto.exitTime?.Length > 60 ? invoiceDto.exitTime?.Substring(0, 60) : invoiceDto.exitTime;
            paymentConditionName = invoiceDto.paymentConditionName?.Length > 60 ? invoiceDto.paymentConditionName?.Substring(0, 60) : invoiceDto.paymentConditionName;
            sellerCpf = invoiceDto.sellerCpf?.Length > 14 ? invoiceDto.sellerCpf?.Substring(0, 14) : invoiceDto.sellerCpf;
            observationNFe = invoiceDto.observationNFE?.Length > 8000 ? invoiceDto.observationNFE?.Substring(0, 8000) : invoiceDto.observationNFE;

            Responses.Add(invoiceDto.invoiceSequence, JsonConvert.SerializeObject(invoiceDto));

            invoiceDto.salesOrder?.ForEach(saleOrder =>
                salesOrder.Add(new SaleOrder(saleOrder, invoiceSequence)
            ));

            invoiceDto.items?.ForEach(item =>
                items.Add(new ProductItem(item, invoiceSequence)
            ));

            invoiceDto.payments?.ForEach(payment =>
                payments.Add(new Payment(payment, invoiceSequence)
            ));

            invoiceDto.observationNF?.ForEach(observation =>
                observationNF.Add(new ObservationNF(observation, invoiceSequence)
            ));

            invoiceDto.referencedTaxInvoice?.ForEach(referencedTax =>
                referencedTaxInvoice.Add(new ReferencedTax(referencedTax, invoiceSequence)
            ));

            //eletronic
            receipt = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.eletronic?.receipt);
            disableProtocol = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.eletronic?.disableProtocol);

            receivementDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.eletronic?.receivementDate);
            disableDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(invoiceDto.eletronic?.disableDate);

            accessKey = invoiceDto.eletronic?.accessKey?.Length > 44 ? invoiceDto.eletronic?.accessKey?.Substring(0, 44) : invoiceDto.eletronic?.accessKey; 
            electronicInvoiceStatus = invoiceDto.eletronic?.electronicInvoiceStatus?.Length > 60 ? invoiceDto.eletronic?.electronicInvoiceStatus?.Substring(0, 60) : invoiceDto.eletronic?.electronicInvoiceStatus;

            //ecf
            ecfCoupon = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.ecf?.coupon);

            ecfSerialMachine = invoiceDto.ecf?.serialMachine?.Length > 60 ? invoiceDto.ecf?.serialMachine?.Substring(0, 60) : invoiceDto.ecf?.serialMachine;

            //sat
            satCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.sat?.satCode);
            satDvNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.sat?.dvNumber);
            satSequence = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.sat?.satSequence);

            satCnfNumber = invoiceDto.sat?.cnfNumber?.Length > 60 ? invoiceDto.sat?.cnfNumber?.Substring(0, 60) : invoiceDto.sat?.cnfNumber;
            satCancellationKey = invoiceDto.sat?.cancellationKey?.Length > 60 ? invoiceDto.sat?.cancellationKey?.Substring(0, 60) : invoiceDto.sat?.cancellationKey; 
            satSerialMachine = invoiceDto.sat?.serialMachine?.Length > 60 ? invoiceDto.sat?.serialMachine?.Substring(0, 60) : invoiceDto.sat?.serialMachine;

            //shipping company
            shippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.shippingCompany?.shippingCompanyCode);
            shippingCompanyRedispatchCarrierCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.shippingCompany?.redispatchCarrierCode);
            shippingCompanyCityCodeRedispatch = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.shippingCompany?.cityCodeRedispatch);
            shippingCompanyPublicPlaceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.shippingCompany?.publicPlaceCode);
            shippingCompanyPackageNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.shippingCompany?.packageNumber);

            shippingCompanyFreightValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.shippingCompany?.freightValue);
            shippingCompanyGrossWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.shippingCompany?.grossWeight);
            shippingCompanyNetWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(invoiceDto.shippingCompany?.netWeight);
            
            shippingCompanyName = invoiceDto.shippingCompany?.shippingCompanyName?.Length > 250 ? invoiceDto.shippingCompany?.shippingCompanyName?.Substring(0, 250) : invoiceDto.shippingCompany?.shippingCompanyName;
            shippingCompanyFreitghtType = invoiceDto.shippingCompany?.freitghtType?.Length > 60 ? invoiceDto.shippingCompany?.freitghtType?.Substring(0, 60) : invoiceDto.shippingCompany?.freitghtType;
            shippingCompanyFreitghtTypeRedispatch = invoiceDto.shippingCompany?.freitghtTypeRedispatch?.Length > 60 ? invoiceDto.shippingCompany?.freitghtTypeRedispatch?.Substring(0, 60) : invoiceDto.shippingCompany?.freitghtTypeRedispatch;
            shippingCompanyRedispatchCarrierName = invoiceDto.shippingCompany?.redispatchCarrierName?.Length > 250 ? invoiceDto.shippingCompany?.redispatchCarrierName?.Substring(0, 250) : invoiceDto.shippingCompany?.redispatchCarrierName;
            shippingCompanyCityNameRedispatch = invoiceDto.shippingCompany?.cityNameRedispatch?.Length > 60 ? invoiceDto.shippingCompany?.cityNameRedispatch?.Substring(0, 60) : invoiceDto.shippingCompany?.cityNameRedispatch;
            shippingCompanyCityUfRedispatch = invoiceDto.shippingCompany?.cityUfRedispatch?.Length > 20 ? invoiceDto.shippingCompany?.cityUfRedispatch?.Substring(0, 20) : invoiceDto.shippingCompany?.cityUfRedispatch;
            shippingCompanyType = invoiceDto.shippingCompany?.shippingCompanyType?.Length > 60 ? invoiceDto.shippingCompany?.shippingCompanyType?.Substring(0, 60) : invoiceDto.shippingCompany?.shippingCompanyType;
            shippingCompanyPublicPlaceTypeName = invoiceDto.shippingCompany?.publicPlaceTypeName?.Length > 60 ? invoiceDto.shippingCompany?.publicPlaceTypeName?.Substring(0, 60) : invoiceDto.shippingCompany?.publicPlaceTypeName;
            shippingCompanyPublicPlaceName = invoiceDto.shippingCompany?.publicPlaceName?.Length > 60 ? invoiceDto.shippingCompany?.publicPlaceName?.Substring(0, 60) : invoiceDto.shippingCompany?.publicPlaceName;
            shippingCompanyStateAbbreviation = invoiceDto.shippingCompany?.stateAbbreviation?.Length > 20 ? invoiceDto.shippingCompany?.stateAbbreviation?.Substring(0, 20) : invoiceDto.shippingCompany?.stateAbbreviation;
            shippingCompanyCep = invoiceDto.shippingCompany?.cep?.Length > 9 ? invoiceDto.shippingCompany?.cep?.Substring(0, 9) : invoiceDto.shippingCompany?.cep;
            shippingCompanyPostOfficeBox = invoiceDto.shippingCompany?.postOfficeBox?.Length > 60 ? invoiceDto.shippingCompany?.postOfficeBox?.Substring(0, 60) : invoiceDto.shippingCompany?.postOfficeBox;
            shippingCompanyNumberStateRegistration = invoiceDto.shippingCompany?.numberStateRegistration?.Length > 60 ? invoiceDto.shippingCompany?.numberStateRegistration?.Substring(0, 60) : invoiceDto.shippingCompany?.numberStateRegistration;
            shippingCompanyCpfCnpj = invoiceDto.shippingCompany?.cpfCnpj?.Length > 14 ? invoiceDto.shippingCompany?.cpfCnpj?.Substring(0, 14) : invoiceDto.shippingCompany?.cpfCnpj;
            shippingCompanyPlaqueCode = invoiceDto.shippingCompany?.plaqueCode?.Length > 60 ? invoiceDto.shippingCompany?.plaqueCode?.Substring(0, 60) : invoiceDto.shippingCompany?.plaqueCode;
            shippingCompanyPlaqueUf = invoiceDto.shippingCompany?.plaqueUf?.Length > 20 ? invoiceDto.shippingCompany?.plaqueUf?.Substring(0, 20) : invoiceDto.shippingCompany?.plaqueUf;
            shippingCompanySpecies = invoiceDto.shippingCompany?.species?.Length > 60 ? invoiceDto.shippingCompany?.species?.Substring(0, 60) : invoiceDto.shippingCompany?.species;
            shippingCompanyBrandName = invoiceDto.shippingCompany?.brandName?.Length > 60 ? invoiceDto.shippingCompany?.brandName?.Substring(0, 60) : invoiceDto.shippingCompany?.brandName;
            shippingCompanyTrackingCode = invoiceDto.shippingCompany?.trackingCode?.Length > 60 ? invoiceDto.shippingCompany?.trackingCode?.Substring(0, 60) : invoiceDto.shippingCompany?.trackingCode;
            shippingCompanyNeighborhood = invoiceDto.shippingCompany?.neighborhood?.Length > 60 ? invoiceDto.shippingCompany?.neighborhood?.Substring(0, 60) : invoiceDto.shippingCompany?.neighborhood;
            shippingCompanyCityName = invoiceDto.shippingCompany?.cityName?.Length > 40 ? invoiceDto.shippingCompany?.cityName?.Substring(0, 40) : invoiceDto.shippingCompany?.cityName;

            //person
            personType = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.person.personType);
            personPostOfficeBox = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.person.postOfficeBox);
            personAddressNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.person.addressNumber);
            personCityCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.person.cityCode);

            personCep = invoiceDto.person?.cep?.Length > 250 ? invoiceDto.person?.cep?.Substring(0, 250) : invoiceDto.person?.cep;
            personAddress = invoiceDto.person?.address?.Length > 250 ? invoiceDto.person?.address?.Substring(0, 250) : invoiceDto.person?.address;
            personPublicPlace = invoiceDto.person?.publicPlace?.Length > 250 ? invoiceDto.person?.publicPlace?.Substring(0, 250) : invoiceDto.person?.publicPlace;
            personComplement = invoiceDto.person?.complement?.Length > 250 ? invoiceDto.person?.complement?.Substring(0, 250) : invoiceDto.person?.complement;
            personNeighborhood = invoiceDto.person?.neighborhood?.Length > 250 ? invoiceDto.person?.neighborhood?.Substring(0, 250) : invoiceDto.person?.neighborhood;
            personCity = invoiceDto.person?.city?.Length > 250 ? invoiceDto.person?.city?.Substring(0, 250) : invoiceDto.person?.city;
            personStateAbbreviation = invoiceDto.person?.stateAbbreviation?.Length > 250 ? invoiceDto.person?.stateAbbreviation?.Substring(0, 250) : invoiceDto.person?.stateAbbreviation;
            personFoneNumber = invoiceDto.person?.foneNumber?.Length > 250 ? invoiceDto.person?.foneNumber?.Substring(0, 250) : invoiceDto.person?.foneNumber;
            personRgIe = invoiceDto.person?.rgIe?.Length > 250 ? invoiceDto.person?.rgIe?.Substring(0, 250) : invoiceDto.person?.rgIe;
            personCpfCnpj = invoiceDto.person?.personCpfCnpj?.Length > 250 ? invoiceDto.person?.personCpfCnpj?.Substring(0, 250) : invoiceDto.person?.personCpfCnpj;

            //production order
            productionOrderBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.productionOrder?.branchCode);
            productionOrderCycleCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.productionOrder?.cycleCode);
            productionOrderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(invoiceDto.productionOrder?.orderCode);

            productionOrderRelationshipType = invoiceDto.productionOrder?.orderRelationshipType?.Length > 250 ? invoiceDto.productionOrder?.orderRelationshipType?.Substring(0, 250) : invoiceDto.productionOrder?.orderRelationshipType;
        }
    }
}
