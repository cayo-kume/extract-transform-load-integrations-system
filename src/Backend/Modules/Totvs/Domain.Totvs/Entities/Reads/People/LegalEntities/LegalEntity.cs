using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.LegalEntities
{
    /// <summary>
    /// pessoa jurídica (POST person/v2/legal-entities/search), tabela totvs.TotvsLegalEntities. Tamanhos do swagger da API Person
    /// </summary>
    public class LegalEntity
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string cnpj { get; private set; } = String.Empty;
        public bool? isInactive { get; private set; }
        public int? branchInsertCode { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string fantasyName { get; private set; } = String.Empty;
        public string uf { get; private set; } = String.Empty;
        public string numberStateRegistration { get; private set; } = String.Empty;
        public int? suframaCode { get; private set; }
        public string dateFoundation { get; private set; } = String.Empty;
        public int? codeActivity { get; private set; }
        public string numberEmployees { get; private set; } = String.Empty;
        public decimal? monthlyInvoicing { get; private set; }
        public decimal? shareCapital { get; private set; }
        public string homePage { get; private set; } = String.Empty;
        public int? codeActivityCnae { get; private set; }
        public int? codeActivityCnae2 { get; private set; }
        public string typeTaxRegime { get; private set; } = String.Empty;
        public string typeSubTributary { get; private set; } = String.Empty;
        public string typeDescriptionSuFrama { get; private set; } = String.Empty;
        public string registrationMunicipal { get; private set; } = String.Empty;
        public string descriptionJuntaCial { get; private set; } = String.Empty;
        public DateTime? dateRegJuntaCial { get; private set; }
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
        public string customerStatus { get; private set; } = String.Empty;
        public string status { get; private set; } = String.Empty;

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
        public List<Persons.Partner> partners { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Contact> contacts { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Statistic> statistics { get; private set; } = [];

        [SkipProperty]
        public List<Persons.SocialNetwork> socialNetworks { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Representative> representatives { get; private set; } = [];

        [SkipProperty]
        public List<Persons.PaymentMethod> paymentMethods { get; private set; } = [];

        public LegalEntity() { }

        public LegalEntity(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.LegalEntities.LegalEntity dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            cnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cnpj, 14);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);
            branchInsertCode = NullableConverters.ToNullableInt32(dto.branchInsertCode);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            fantasyName = ConverterValidatorsExtensions.StringLengthValidation(dto.fantasyName, 60);
            uf = ConverterValidatorsExtensions.StringLengthValidation(dto.uf, 2);
            numberStateRegistration = ConverterValidatorsExtensions.StringLengthValidation(dto.numberStateRegistration, 18);
            suframaCode = NullableConverters.ToNullableInt32(dto.suframaCode);
            dateFoundation = ConverterValidatorsExtensions.StringLengthValidation(dto.dateFoundation, 20);
            codeActivity = NullableConverters.ToNullableInt32(dto.codeActivity);
            numberEmployees = ConverterValidatorsExtensions.StringLengthValidation(dto.numberEmployees, 40);
            monthlyInvoicing = NullableConverters.ToNullableDecimal(dto.monthlyInvoicing);
            shareCapital = NullableConverters.ToNullableDecimal(dto.shareCapital);
            homePage = ConverterValidatorsExtensions.StringLengthValidation(dto.homePage, 100);
            codeActivityCnae = NullableConverters.ToNullableInt32(dto.codeActivityCnae);
            codeActivityCnae2 = NullableConverters.ToNullableInt32(dto.codeActivityCnae2);
            typeTaxRegime = ConverterValidatorsExtensions.StringLengthValidation(dto.typeTaxRegime, 40);
            typeSubTributary = ConverterValidatorsExtensions.StringLengthValidation(dto.typeSubTributary, 40);
            typeDescriptionSuFrama = ConverterValidatorsExtensions.StringLengthValidation(dto.typeDescriptionSuFrama, 40);
            registrationMunicipal = ConverterValidatorsExtensions.StringLengthValidation(dto.registrationMunicipal, 18);
            descriptionJuntaCial = ConverterValidatorsExtensions.StringLengthValidation(dto.descriptionJuntaCial, 20);
            dateRegJuntaCial = NullableConverters.ToNullableDateTime(dto.dateRegJuntaCial);
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
            customerStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.customerStatus, 20);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 20);

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

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real, docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.5)
            addresses.AddRange((dto.addresses ?? []).Select((x, i) => new Persons.Address(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.addressTypeCode, x.sequenceCode)).Select(g => g.Last()));
            phones.AddRange((dto.phones ?? []).Select((x, i) => new Persons.Phone(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            emails.AddRange((dto.emails ?? []).Select((x, i) => new Persons.Email(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            observations.AddRange((dto.observations ?? []).Select((x, i) => new Persons.Observation(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            customerObservations.AddRange((dto.customerObservations ?? []).Select((x, i) => new Persons.CustomerObservation(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            additionalFields.AddRange((dto.additionalFields ?? []).Select((x, i) => new Persons.AdditionalField(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            classifications.AddRange((dto.classifications ?? []).Select((x, i) => new Persons.Classification(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.typeCode, x.code.ToUpperInvariant())).Select(g => g.Last()));
            references.AddRange((dto.references ?? []).Select((x, i) => new Persons.Reference(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            relateds.AddRange((dto.relateds ?? []).Select((x, i) => new Persons.Related(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            partners.AddRange((dto.partners ?? []).Select((x, i) => new Persons.Partner(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            contacts.AddRange((dto.contacts ?? []).Select((x, i) => new Persons.Contact(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            statistics.AddRange((dto.statistics ?? []).Select((x, i) => new Persons.Statistic(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.branchCode, x.customerCode)).Select(g => g.Last()));
            socialNetworks.AddRange((dto.socialNetworks ?? []).Select((x, i) => new Persons.SocialNetwork(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            representatives.AddRange((dto.representatives ?? []).Select((x, i) => new Persons.Representative(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.representativeCode).Select(g => g.Last()));
            paymentMethods.AddRange((dto.paymentMethods ?? []).Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => new Persons.PaymentMethod(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.paymentMethod).Select(g => g.Last()));
        }
    }
}
