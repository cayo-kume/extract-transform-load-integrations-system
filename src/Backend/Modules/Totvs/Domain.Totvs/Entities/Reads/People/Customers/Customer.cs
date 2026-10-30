using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Customers
{
    /// <summary>
    /// pessoa física (POST person/v2/individuals/search), tabela totvs.TotvsPeople. Tamanhos do swagger da API Person
    /// </summary>
    public class Customer
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string cpf { get; private set; } = String.Empty;
        public bool? isInactive { get; private set; }
        public string maritalStatus { get; private set; } = String.Empty;
        public string gender { get; private set; } = String.Empty;
        public long? ctps { get; private set; }
        public long? ctpsSerial { get; private set; }
        public string rg { get; private set; } = String.Empty;
        public string rgFederalAgency { get; private set; } = String.Empty;
        public int? suframaCode { get; private set; }
        public string motherName { get; private set; } = String.Empty;
        public string fatherName { get; private set; } = String.Empty;
        public string nationality { get; private set; } = String.Empty;
        public DateTime? birthDate { get; private set; }
        public decimal? monthlyIncome { get; private set; }
        public string occupation { get; private set; } = String.Empty;
        public DateTime? hireDate { get; private set; }
        public string workPlace { get; private set; } = String.Empty;
        public string homeTown { get; private set; } = String.Empty;
        public int branchInsertCode { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string homePage { get; private set; } = String.Empty;
        public int? codeMainRelated { get; private set; }
        public string cpfCnpjMainRelated { get; private set; } = String.Empty;
        public string nameMainRelated { get; private set; } = String.Empty;
        public DateTime? insertDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }
        public bool? isCustomer { get; private set; }
        public bool? isSupplier { get; private set; }
        public bool? isRepresentative { get; private set; }
        public bool? isPurchasingGuide { get; private set; }
        public bool? isShippingCompany { get; private set; }
        public bool? isEmployee { get; private set; }
        public string employeeStatus { get; private set; } = String.Empty;
        public string customerStatus { get; private set; } = String.Empty;

        // shippingCompany (1:1): colunas com prefixo, nulas quando o objeto não vem
        public decimal? shippingCompanyPercentegeValue { get; private set; }
        public decimal? shippingCompanyKilogramsValue { get; private set; }
        public decimal? shippingCompanyMinimunValue { get; private set; }
        public decimal? shippingCompanyMaximumValue { get; private set; }
        public string shippingCompanyAnttCode { get; private set; } = String.Empty;
        public string shippingCompanyTypeDeductIcms { get; private set; } = String.Empty;

        // preferences (1:1): colunas com prefixo, nulas quando o objeto não vem
        public int? preferencesPaymentConditionCode { get; private set; }
        public string preferencesPaymentConditionDescription { get; private set; } = String.Empty;
        public int? preferencesShippingCompanyCode { get; private set; }
        public string preferencesShippingCompanyName { get; private set; } = String.Empty;
        public string preferencesFreightType { get; private set; } = String.Empty;
        public int? preferencesBridgeShippingCompanyCode { get; private set; }
        public string preferencesBridgeShippingCompanyName { get; private set; } = String.Empty;
        public int? preferencesMaximumAverageTerm { get; private set; }
        public int? preferencesPriceTableCode { get; private set; }
        public string preferencesPriceTableDescription { get; private set; } = String.Empty;
        public int? preferencesOrderPriority { get; private set; }
        public int? preferencesBearerCode { get; private set; }
        public string preferencesBearerName { get; private set; } = String.Empty;

        // individualAdditionals (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string individualAdditionalsEducationLevel { get; private set; } = String.Empty;
        public int? individualAdditionalsNumberOfChildren { get; private set; }
        public int? individualAdditionalsNumberOfDependents { get; private set; }
        public int? individualAdditionalsResidenceMonths { get; private set; }
        public int? individualAdditionalsWorkMonths { get; private set; }
        public string individualAdditionalsPreviousWorkPlace { get; private set; } = String.Empty;
        public DateTime? individualAdditionalsResidingSinceDate { get; private set; }
        public string individualAdditionalsHomeType { get; private set; } = String.Empty;
        public string individualAdditionalsCarType { get; private set; } = String.Empty;
        public decimal? individualAdditionalsScore { get; private set; }
        public decimal? individualAdditionalsRisk { get; private set; }
        public DateTime? individualAdditionalsSpcUpdateDate { get; private set; }
        public bool? individualAdditionalsIsStreetVendor { get; private set; }
        public string individualAdditionalsStreetVendorNumber { get; private set; } = String.Empty;
        public int? individualAdditionalsRgType { get; private set; }
        public DateTime? individualAdditionalsRgIssueDate { get; private set; }
        public string individualAdditionalsPassportNumber { get; private set; } = String.Empty;
        public int? individualAdditionalsPassportCountryCode { get; private set; }
        public string individualAdditionalsDriverLicenseNumber { get; private set; } = String.Empty;
        public DateTime? individualAdditionalsDriverLicenseExpirationDate { get; private set; }
        public DateTime? individualAdditionalsFirstDriverLicenseDate { get; private set; }
        public string individualAdditionalsDriverLicenseAgency { get; private set; } = String.Empty;
        public string individualAdditionalsDriverLicenseCategory { get; private set; } = String.Empty;
        public string individualAdditionalsVoterRegistrationNumber { get; private set; } = String.Empty;
        public string individualAdditionalsVoterZone { get; private set; } = String.Empty;
        public string individualAdditionalsVoterSection { get; private set; } = String.Empty;
        public string individualAdditionalsMilitaryCertificateType { get; private set; } = String.Empty;
        public string individualAdditionalsMilitaryCategory { get; private set; } = String.Empty;
        public string individualAdditionalsMilitaryDispensation { get; private set; } = String.Empty;
        public string individualAdditionalsNitNumber { get; private set; } = String.Empty;
        public string individualAdditionalsCtpsState { get; private set; } = String.Empty;
        public bool? individualAdditionalsIsInformalWorker { get; private set; }
        public string individualAdditionalsPisNumber { get; private set; } = String.Empty;
        public DateTime? individualAdditionalsPisIssueDate { get; private set; }
        public string individualAdditionalsPisState { get; private set; } = String.Empty;
        public string individualAdditionalsRuralProducerRegistration { get; private set; } = String.Empty;
        public string individualAdditionalsRuralProducerState { get; private set; } = String.Empty;
        public string individualAdditionalsCifIssuer { get; private set; } = String.Empty;
        public string individualAdditionalsCifState { get; private set; } = String.Empty;
        public string individualAdditionalsCifNumber { get; private set; } = String.Empty;
        public string individualAdditionalsBirthState { get; private set; } = String.Empty;
        public string individualAdditionalsPassportIssuer { get; private set; } = String.Empty;
        public string individualAdditionalsPassportState { get; private set; } = String.Empty;
        public DateTime? individualAdditionalsPassportIssueDate { get; private set; }
        public DateTime? individualAdditionalsPassportExpirationDate { get; private set; }
        public string individualAdditionalsRicNumber { get; private set; } = String.Empty;
        public string individualAdditionalsRicState { get; private set; } = String.Empty;
        public string individualAdditionalsRicIssuer { get; private set; } = String.Empty;
        public int? individualAdditionalsRicCityCode { get; private set; }
        public DateTime? individualAdditionalsRicIssueDate { get; private set; }
        public DateTime? individualAdditionalsRicExpirationDate { get; private set; }
        public string individualAdditionalsForeignDocumentNumber { get; private set; } = String.Empty;
        public string individualAdditionalsNickname { get; private set; } = String.Empty;
        public string individualAdditionalsCrcNumber { get; private set; } = String.Empty;
        public string individualAdditionalsDriverLicenseSecurityNumber { get; private set; } = String.Empty;
        public string individualAdditionalsDriverLicenseRegistrationNumber { get; private set; } = String.Empty;
        public DateTime? individualAdditionalsCrcRegistrationDate { get; private set; }
        public DateTime? individualAdditionalsCrcExpirationDate { get; private set; }
        public string individualAdditionalsSocialName { get; private set; } = String.Empty;
        public int? individualAdditionalsHomeTown { get; private set; }   // int32 (código da cidade); homeTown da pessoa é o nome

        [SkipProperty]
        public List<Persons.Address> addresses { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Phone> phones { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Email> emails { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Observation> observations { get; private set; } = [];

        [SkipProperty]
        public List<Persons.CustomerObservation> customerObservations { get; private set; } = [];

        [SkipProperty]
        public List<Persons.AdditionalField> additionalFields { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Classification> classifications { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Reference> references { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Related> relateds { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Statistic> statistics { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Representative> representatives { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Familiar> familiars { get; private set; } = [];

        public Customer() { }

        public Customer(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Customers.Customer dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            cpf = ConverterValidatorsExtensions.StringLengthValidation(dto.cpf, 11);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);
            maritalStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.maritalStatus, 20);
            gender = ConverterValidatorsExtensions.StringLengthValidation(dto.gender, 20);
            ctps = (String.IsNullOrWhiteSpace(dto.ctps) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.ctps));
            ctpsSerial = (String.IsNullOrWhiteSpace(dto.ctpsSerial) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.ctpsSerial));
            rg = ConverterValidatorsExtensions.StringLengthValidation(dto.rg, 20);
            rgFederalAgency = ConverterValidatorsExtensions.StringLengthValidation(dto.rgFederalAgency, 10);
            suframaCode = NullableConverters.ToNullableInt32(dto.suframaCode);
            motherName = ConverterValidatorsExtensions.StringLengthValidation(dto.motherName, 60);
            fatherName = ConverterValidatorsExtensions.StringLengthValidation(dto.fatherName, 60);
            nationality = ConverterValidatorsExtensions.StringLengthValidation(dto.nationality, 30);
            birthDate = NullableConverters.ToNullableDateTime(dto.birthDate);
            monthlyIncome = NullableConverters.ToNullableDecimal(dto.monthlyIncome);
            occupation = ConverterValidatorsExtensions.StringLengthValidation(dto.occupation, 60);
            hireDate = NullableConverters.ToNullableDateTime(dto.hireDate);
            workPlace = ConverterValidatorsExtensions.StringLengthValidation(dto.workPlace, 60);
            homeTown = ConverterValidatorsExtensions.StringLengthValidation(dto.homeTown, 60);
            branchInsertCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchInsertCode);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            homePage = ConverterValidatorsExtensions.StringLengthValidation(dto.homePage, 100);
            codeMainRelated = NullableConverters.ToNullableInt32(dto.codeMainRelated);
            cpfCnpjMainRelated = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpjMainRelated, 14);
            nameMainRelated = ConverterValidatorsExtensions.StringLengthValidation(dto.nameMainRelated, 60);
            insertDate = NullableConverters.ToNullableDateTime(dto.insertDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            isCustomer = NullableConverters.ToNullableBoolean(dto.isCustomer);
            isSupplier = NullableConverters.ToNullableBoolean(dto.isSupplier);
            isRepresentative = NullableConverters.ToNullableBoolean(dto.isRepresentative);
            isPurchasingGuide = NullableConverters.ToNullableBoolean(dto.isPurchasingGuide);
            isShippingCompany = NullableConverters.ToNullableBoolean(dto.isShippingCompany);
            isEmployee = NullableConverters.ToNullableBoolean(dto.isEmployee);
            employeeStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.employeeStatus, 20);
            customerStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.customerStatus, 20);

            shippingCompanyPercentegeValue = NullableConverters.ToNullableDecimal(dto.shippingCompany?.percentegeValue);
            shippingCompanyKilogramsValue = NullableConverters.ToNullableDecimal(dto.shippingCompany?.kilogramsValue);
            shippingCompanyMinimunValue = NullableConverters.ToNullableDecimal(dto.shippingCompany?.minimunValue);
            shippingCompanyMaximumValue = NullableConverters.ToNullableDecimal(dto.shippingCompany?.maximumValue);
            shippingCompanyAnttCode = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.anttCode, 80);
            shippingCompanyTypeDeductIcms = ConverterValidatorsExtensions.StringLengthValidation(dto.shippingCompany?.typeDeductIcms, 20);
            preferencesPaymentConditionCode = NullableConverters.ToNullableInt32(dto.preferences?.paymentConditionCode);
            preferencesPaymentConditionDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.paymentConditionDescription, 40);
            preferencesShippingCompanyCode = NullableConverters.ToNullableInt32(dto.preferences?.shippingCompanyCode);
            preferencesShippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.shippingCompanyName, 60);
            preferencesFreightType = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.freightType, 30);
            preferencesBridgeShippingCompanyCode = NullableConverters.ToNullableInt32(dto.preferences?.bridgeShippingCompanyCode);
            preferencesBridgeShippingCompanyName = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.bridgeShippingCompanyName, 60);
            preferencesMaximumAverageTerm = NullableConverters.ToNullableInt32(dto.preferences?.maximumAverageTerm);
            preferencesPriceTableCode = NullableConverters.ToNullableInt32(dto.preferences?.priceTableCode);
            preferencesPriceTableDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.priceTableDescription, 40);
            preferencesOrderPriority = NullableConverters.ToNullableInt32(dto.preferences?.orderPriority);
            preferencesBearerCode = NullableConverters.ToNullableInt32(dto.preferences?.bearerCode);
            preferencesBearerName = ConverterValidatorsExtensions.StringLengthValidation(dto.preferences?.bearerName, 40);
            individualAdditionalsEducationLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.educationLevel, 40);
            individualAdditionalsNumberOfChildren = NullableConverters.ToNullableInt32(dto.individualAdditionals?.numberOfChildren);
            individualAdditionalsNumberOfDependents = NullableConverters.ToNullableInt32(dto.individualAdditionals?.numberOfDependents);
            individualAdditionalsResidenceMonths = NullableConverters.ToNullableInt32(dto.individualAdditionals?.residenceMonths);
            individualAdditionalsWorkMonths = NullableConverters.ToNullableInt32(dto.individualAdditionals?.workMonths);
            individualAdditionalsPreviousWorkPlace = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.previousWorkPlace, 60);
            individualAdditionalsResidingSinceDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.residingSinceDate);
            individualAdditionalsHomeType = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.homeType, 40);
            individualAdditionalsCarType = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.carType, 40);
            individualAdditionalsScore = NullableConverters.ToNullableDecimal(dto.individualAdditionals?.score);
            individualAdditionalsRisk = NullableConverters.ToNullableDecimal(dto.individualAdditionals?.risk);
            individualAdditionalsSpcUpdateDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.spcUpdateDate);
            individualAdditionalsIsStreetVendor = NullableConverters.ToNullableBoolean(dto.individualAdditionals?.isStreetVendor);
            individualAdditionalsStreetVendorNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.streetVendorNumber, 20);
            individualAdditionalsRgType = NullableConverters.ToNullableInt32(dto.individualAdditionals?.rgType);
            individualAdditionalsRgIssueDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.rgIssueDate);
            individualAdditionalsPassportNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.passportNumber, 40);
            individualAdditionalsPassportCountryCode = NullableConverters.ToNullableInt32(dto.individualAdditionals?.passportCountryCode);
            individualAdditionalsDriverLicenseNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.driverLicenseNumber, 20);
            individualAdditionalsDriverLicenseExpirationDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.driverLicenseExpirationDate);
            individualAdditionalsFirstDriverLicenseDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.firstDriverLicenseDate);
            individualAdditionalsDriverLicenseAgency = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.driverLicenseAgency, 20);
            individualAdditionalsDriverLicenseCategory = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.driverLicenseCategory, 40);
            individualAdditionalsVoterRegistrationNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.voterRegistrationNumber, 20);
            individualAdditionalsVoterZone = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.voterZone, 10);
            individualAdditionalsVoterSection = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.voterSection, 10);
            individualAdditionalsMilitaryCertificateType = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.militaryCertificateType, 40);
            individualAdditionalsMilitaryCategory = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.militaryCategory, 40);
            individualAdditionalsMilitaryDispensation = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.militaryDispensation, 20);
            individualAdditionalsNitNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.nitNumber, 20);
            individualAdditionalsCtpsState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ctpsState, 2);
            individualAdditionalsIsInformalWorker = NullableConverters.ToNullableBoolean(dto.individualAdditionals?.isInformalWorker);
            individualAdditionalsPisNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.pisNumber, 20);
            individualAdditionalsPisIssueDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.pisIssueDate);
            individualAdditionalsPisState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.pisState, 2);
            individualAdditionalsRuralProducerRegistration = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ruralProducerRegistration, 18);
            individualAdditionalsRuralProducerState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ruralProducerState, 2);
            individualAdditionalsCifIssuer = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.cifIssuer, 20);
            individualAdditionalsCifState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.cifState, 2);
            individualAdditionalsCifNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.cifNumber, 20);
            individualAdditionalsBirthState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.birthState, 2);
            individualAdditionalsPassportIssuer = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.passportIssuer, 10);
            individualAdditionalsPassportState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.passportState, 2);
            individualAdditionalsPassportIssueDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.passportIssueDate);
            individualAdditionalsPassportExpirationDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.passportExpirationDate);
            individualAdditionalsRicNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ricNumber, 20);
            individualAdditionalsRicState = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ricState, 2);
            individualAdditionalsRicIssuer = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.ricIssuer, 20);
            individualAdditionalsRicCityCode = NullableConverters.ToNullableInt32(dto.individualAdditionals?.ricCityCode);
            individualAdditionalsRicIssueDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.ricIssueDate);
            individualAdditionalsRicExpirationDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.ricExpirationDate);
            individualAdditionalsForeignDocumentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.foreignDocumentNumber, 20);
            individualAdditionalsNickname = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.nickname, 40);
            individualAdditionalsCrcNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.crcNumber, 20);
            individualAdditionalsDriverLicenseSecurityNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.driverLicenseSecurityNumber, 20);
            individualAdditionalsDriverLicenseRegistrationNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.driverLicenseRegistrationNumber, 20);
            individualAdditionalsCrcRegistrationDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.crcRegistrationDate);
            individualAdditionalsCrcExpirationDate = NullableConverters.ToNullableDateTime(dto.individualAdditionals?.crcExpirationDate);
            individualAdditionalsSocialName = ConverterValidatorsExtensions.StringLengthValidation(dto.individualAdditionals?.socialName, 70);
            individualAdditionalsHomeTown = NullableConverters.ToNullableInt32(dto.individualAdditionals?.homeTown);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real, src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.5)
            addresses.AddRange((dto.addresses ?? []).Select((x, i) => new Persons.Address(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.addressTypeCode, x.sequenceCode)).Select(g => g.Last()));
            phones.AddRange((dto.phones ?? []).Select((x, i) => new Persons.Phone(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            emails.AddRange((dto.emails ?? []).Select((x, i) => new Persons.Email(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            observations.AddRange((dto.observations ?? []).Select((x, i) => new Persons.Observation(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            customerObservations.AddRange((dto.customerObservations ?? []).Select((x, i) => new Persons.CustomerObservation(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            additionalFields.AddRange((dto.additionalFields ?? []).Select((x, i) => new Persons.AdditionalField(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            classifications.AddRange((dto.classifications ?? []).Select((x, i) => new Persons.Classification(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.typeCode, x.code.ToUpperInvariant())).Select(g => g.Last()));
            references.AddRange((dto.references ?? []).Select((x, i) => new Persons.Reference(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            relateds.AddRange((dto.relateds ?? []).Select((x, i) => new Persons.Related(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            statistics.AddRange((dto.statistics ?? []).Select((x, i) => new Persons.Statistic(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.branchCode, x.customerCode)).Select(g => g.Last()));
            representatives.AddRange((dto.representatives ?? []).Select((x, i) => new Persons.Representative(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.representativeCode).Select(g => g.Last()));
            familiars.AddRange((dto.familiars ?? []).Select((x, i) => new Persons.Familiar(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
        }
    }
}
