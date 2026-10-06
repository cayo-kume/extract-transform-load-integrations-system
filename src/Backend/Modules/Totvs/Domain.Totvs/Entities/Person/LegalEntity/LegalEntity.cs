using Domain.Shared.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class LegalEntity
    {
        public int code { get; private set; }
        public string cnpj { get; private set; }
        public bool isInactive { get; private set; }
        public int branchInsertCode { get; private set; }
        public string name { get; private set; }
        public string fantasyName { get; private set; }
        public string uf { get; private set; }
        public string numberStateRegistration { get; private set; }
        public int suframaCode { get; private set; }
        public string dateFoundation { get; private set; }
        public int codeActivity { get; private set; }
        public string numberEmployees { get; private set; }
        public int monthlyInvoicing { get; private set; }
        public int shareCapital { get; private set; }
        public string homePage { get; private set; }
        public int codeActivityCnae { get; private set; }
        public int codeActivityCnae2 { get; private set; }
        public string typeTaxRegime { get; private set; }
        public string typeSubTributary { get; private set; }
        public string typeDescriptionSuFrama { get; private set; }
        public string registrationMunicipal { get; private set; }
        public string descriptionJuntaCial { get; private set; }
        public DateTime dateRegJuntaCial { get; private set; }
        public int codeMainRelated { get; private set; }
        public string cpfCnpjMainRelated { get; private set; }
        public string nameMainRelated { get; private set; }
        public DateTime insertDate { get; private set; }
        public DateTime maxChangeFilterDate { get; private set; }
        public bool isCustomer { get; private set; }
        public bool isSupplier { get; private set; }
        public bool isRepresentative { get; private set; }
        public bool isPurchasingGuide { get; private set; }
        public bool isShippingCompany { get; private set; }
        public string customerStatus { get; private set; }
        public string status { get; private set; }
        public int paymentConditionCode { get; private set; }
        public string paymentConditionDescription { get; private set; }
        public int shippingCompanyCode { get; private set; }
        public string shippingCompanyName { get; private set; }
        public string freightType { get; private set; }
        public int bridgeShippingCompanyCode { get; private set; }
        public string bridgeShippingCompanyName { get; private set; }
        public int maximumAverageTerm { get; private set; }
        public int priceTableCode { get; private set; }
        public string priceTableDescription { get; private set; }
        public int orderPriority { get; private set; }
        public int bearerCode { get; private set; }
        public string bearerName { get; private set; }
        public decimal percentegeValue { get; private set; }
        public decimal kilogramsValue { get; private set; }
        public decimal minimunValue { get; private set; }
        public decimal maximumValue { get; private set; }
        public string anttCode { get; private set; }
        public string typeDeductIcms { get; private set; }

        [SkipProperty]
        public List<Address> addresses { get; private set; } = new List<Address>();

        [SkipProperty]
        public List<Phone> phones { get; private set; } = new List<Phone>();

        [SkipProperty]
        public List<Email> emails { get; private set; } = new List<Email>();

        [SkipProperty]
        public List<Observation> observations { get; private set; } = new List<Observation>();

        [SkipProperty]
        public List<CustomerObservation> customerObservations { get; private set; } = new List<CustomerObservation>();

        [SkipProperty]
        public List<AdditionalField> additionalFields { get; private set; } = new List<AdditionalField>();

        [SkipProperty]
        public List<Classification> classifications { get; private set; } = new List<Classification>();

        [SkipProperty]
        public List<Reference> references { get; private set; } = new List<Reference>();

        [SkipProperty]
        public List<Related> relateds { get; private set; } = new List<Related>();

        [SkipProperty]
        public List<Partner> partners { get; private set; } = new List<Partner>();

        [SkipProperty]
        public List<Contact> contacts { get; private set; } = new List<Contact>();

        [SkipProperty]
        public List<Statistic> statistics { get; private set; } = new List<Statistic>();

        [SkipProperty]
        public List<SocialNetwork> socialNetworks { get; private set; } = new List<SocialNetwork>();

        [SkipProperty]
        public List<Representative> representatives { get; private set; } = new List<Representative>();

        [SkipProperty]
        public List<PaymentMethod> paymentMethods { get; private set; } = new List<PaymentMethod>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        public LegalEntity() { }

        public LegalEntity(Domain.Totvs.Dtos.Person.LegalEntity.Item legalEntityDto)
        {
            isInactive = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isInactive);
            isCustomer = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isCustomer);
            isSupplier = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isSupplier);
            isRepresentative = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isRepresentative);
            isPurchasingGuide = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isPurchasingGuide);
            isShippingCompany = ConverterValidatorsExtensions.ConvertToBooleanValidation(legalEntityDto.isShippingCompany);
            
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.code);
            branchInsertCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.branchInsertCode);
            suframaCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.suframaCode);
            codeActivity = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.codeActivity);
            monthlyInvoicing = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.monthlyInvoicing);
            shareCapital = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.shareCapital);
            codeActivityCnae = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.codeActivityCnae);
            codeActivityCnae2 = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.codeActivityCnae2);
            codeMainRelated = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.codeMainRelated);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.paymentConditionCode);
            shippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.shippingCompanyCode);
            bridgeShippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.bridgeShippingCompanyCode);
            maximumAverageTerm = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.maximumAverageTerm);
            priceTableCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.priceTableCode);
            orderPriority = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.orderPriority);
            bearerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(legalEntityDto.preferences?.bearerCode);
            
            dateRegJuntaCial = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(legalEntityDto.dateRegJuntaCial);
            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(legalEntityDto.insertDate);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(legalEntityDto.maxChangeFilterDate);
            
            percentegeValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(legalEntityDto.shippingCompany?.percentegeValue);
            kilogramsValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(legalEntityDto.shippingCompany?.kilogramsValue);
            minimunValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(legalEntityDto.shippingCompany?.minimunValue);
            maximumValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(legalEntityDto.shippingCompany?.maximumValue);
            
            cnpj = legalEntityDto.cnpj?.Length > 60 ? legalEntityDto.cnpj.Substring(0, 60) : legalEntityDto.cnpj;
            name = legalEntityDto.name?.Length > 250 ? legalEntityDto.name.Substring(0, 250) : legalEntityDto.name;
            fantasyName = legalEntityDto.fantasyName?.Length > 250 ? legalEntityDto.fantasyName.Substring(0, 250) : legalEntityDto.fantasyName;
            uf = legalEntityDto.uf?.Length > 20 ? legalEntityDto.uf.Substring(0, 20) : legalEntityDto.uf;
            numberStateRegistration = legalEntityDto.numberStateRegistration?.Length > 60 ? legalEntityDto.numberStateRegistration.Substring(0, 60) : legalEntityDto.numberStateRegistration;
            dateFoundation = legalEntityDto.dateFoundation?.Length > 60 ? legalEntityDto.dateFoundation.Substring(0, 60) : legalEntityDto.dateFoundation;
            numberEmployees = legalEntityDto.numberEmployees?.Length > 60 ? legalEntityDto.numberEmployees.Substring(0, 60) : legalEntityDto.numberEmployees;
            homePage = legalEntityDto.homePage?.Length > 300 ? legalEntityDto.homePage.Substring(0, 300) : legalEntityDto.homePage;
            typeTaxRegime = legalEntityDto.typeTaxRegime?.Length > 60 ? legalEntityDto.typeTaxRegime.Substring(0, 60) : legalEntityDto.typeTaxRegime;
            typeSubTributary = legalEntityDto.typeSubTributary?.Length > 60 ? legalEntityDto.typeSubTributary.Substring(0, 60) : legalEntityDto.typeSubTributary;
            typeDescriptionSuFrama = legalEntityDto.typeDescriptionSuFrama?.Length > 60 ? legalEntityDto.typeDescriptionSuFrama.Substring(0, 60) : legalEntityDto.typeDescriptionSuFrama;
            registrationMunicipal = legalEntityDto.registrationMunicipal?.Length > 60 ? legalEntityDto.registrationMunicipal.Substring(0, 60) : legalEntityDto.registrationMunicipal;
            descriptionJuntaCial = legalEntityDto.descriptionJuntaCial?.Length > 60 ? legalEntityDto.descriptionJuntaCial.Substring(0, 60) : legalEntityDto.descriptionJuntaCial;
            cpfCnpjMainRelated = legalEntityDto.cpfCnpjMainRelated?.Length > 14 ? legalEntityDto.cpfCnpjMainRelated.Substring(0, 14) : legalEntityDto.cpfCnpjMainRelated;
            nameMainRelated = legalEntityDto.nameMainRelated?.Length > 250 ? legalEntityDto.nameMainRelated.Substring(0, 250) : legalEntityDto.nameMainRelated;
            customerStatus = legalEntityDto.customerStatus?.Length > 60 ? legalEntityDto.customerStatus.Substring(0, 60) : legalEntityDto.customerStatus;
            status = legalEntityDto.status?.Length > 60 ? legalEntityDto.status.Substring(0, 60) : legalEntityDto.status;
            paymentConditionDescription = legalEntityDto.preferences?.paymentConditionDescription?.Length > 60 ? legalEntityDto.preferences?.paymentConditionDescription.Substring(0, 60) : legalEntityDto.preferences?.paymentConditionDescription;
            shippingCompanyName = legalEntityDto.preferences?.shippingCompanyName?.Length > 250 ? legalEntityDto.preferences?.shippingCompanyName.Substring(0, 250) : legalEntityDto.preferences?.shippingCompanyName;
            freightType = legalEntityDto.preferences?.freightType?.Length > 60 ? legalEntityDto.preferences?.freightType.Substring(0, 60) : legalEntityDto.preferences?.freightType;
            bridgeShippingCompanyName = legalEntityDto.preferences?.bridgeShippingCompanyName?.Length > 250 ? legalEntityDto.preferences?.bridgeShippingCompanyName.Substring(0, 250) : legalEntityDto.preferences?.bridgeShippingCompanyName;
            priceTableDescription = legalEntityDto.preferences?.priceTableDescription?.Length > 250 ? legalEntityDto.preferences?.priceTableDescription.Substring(0, 250) : legalEntityDto.preferences?.priceTableDescription;
            bearerName = legalEntityDto.preferences?.bearerName?.Length > 60 ? legalEntityDto.preferences?.bearerName.Substring(0, 60) : legalEntityDto.preferences?.bearerName;
            anttCode = legalEntityDto.shippingCompany?.anttCode?.Length > 60 ? legalEntityDto.shippingCompany?.anttCode.Substring(0, 60) : legalEntityDto.shippingCompany?.anttCode;
            typeDeductIcms = legalEntityDto.shippingCompany?.typeDeductIcms?.Length > 60 ? legalEntityDto.shippingCompany?.typeDeductIcms.Substring(0, 60) : legalEntityDto.shippingCompany?.typeDeductIcms;

            Responses.Add(legalEntityDto.code, JsonConvert.SerializeObject(legalEntityDto));

            legalEntityDto.addresses?.ForEach(address =>
                addresses.Add(new Address(address, code)
            ));

            legalEntityDto.phones?.ForEach(phone =>
                phones.Add(new Phone(phone, code)
            ));

            legalEntityDto.emails?.ForEach(email =>
                emails.Add(new Email(email, code)
            ));

            legalEntityDto.observations?.ForEach(observation =>
                observations.Add(new Observation(observation, code)
            ));

            legalEntityDto.customerObservations?.ForEach(customerObservation =>
                customerObservations.Add(new CustomerObservation(customerObservation, code)
            ));

            legalEntityDto.additionalFields?.ForEach(additionalField =>
                additionalFields.Add(new AdditionalField(additionalField, code)
            ));

            legalEntityDto.classifications?.ForEach(classification =>
                classifications.Add(new Classification(classification, code)
            ));

            legalEntityDto.references?.ForEach(reference =>
                references.Add(new Reference(reference, code)
            ));

            legalEntityDto.relateds?.ForEach(related =>
                relateds.Add(new Related(related, code)
            ));

            legalEntityDto.partners?.ForEach(partner =>
                partners.Add(new Partner(partner, code)
            ));

            legalEntityDto.contacts?.ForEach(contact =>
                contacts.Add(new Contact(contact, code)
            ));

            legalEntityDto.statistics?.ForEach(statistic =>
                statistics.Add(new Statistic(statistic, code)
            ));

            legalEntityDto.socialNetworks?.ForEach(socialNetwork =>
                socialNetworks.Add(new SocialNetwork(socialNetwork, code)
            ));

            legalEntityDto.representatives?.ForEach(representative =>
                representatives.Add(new Representative(representative, code)
            ));

            legalEntityDto.paymentMethods?.ForEach(paymentMethod =>
                paymentMethods.Add(new PaymentMethod(paymentMethod, code)
            ));
        }
    }
}
