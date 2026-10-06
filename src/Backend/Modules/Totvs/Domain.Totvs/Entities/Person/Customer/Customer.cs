using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Customer
    {
        public int code { get; private set; }
        public string cpf { get; private set; }
        public bool isInactive { get; private set; }
        public string maritalStatus { get; set; }
        public string gender { get; set; }
        public int ctps { get; set; }
        public int ctpsSerial { get; set; }
        public string rg { get; set; }
        public string rgFederalAgency { get; set; }
        public int suframaCode { get; private set; }
        public string motherName { get; set; }
        public string fatherName { get; set; }
        public string nationality { get; set; }
        public DateTime birthDate { get; set; }
        public int monthlyIncome { get; set; }
        public string occupation { get; set; }
        public DateTime hireDate { get; set; }
        public string workPlace { get; set; }
        public string homeTown { get; set; }
        public int branchInsertCode { get; private set; }
        public string name { get; private set; }
        public string homePage { get; private set; }
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
        public bool isEmployee { get; private set; }
        public string employeeStatus { get; private set; }
        public string customerStatus { get; private set; }
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
        public List<Statistic> statistics { get; private set; } = new List<Statistic>();

        [SkipProperty]
        public List<Familiar> familiars { get; private set; } = new List<Familiar>();

        [SkipProperty]
        public List<Representative> representatives { get; private set; } = new List<Representative>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        public Customer() { }

        public Customer(Domain.Totvs.Dtos.Person.Customer.Item customerDto)
        {
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.code);
            ctps = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.ctps);
            ctpsSerial = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.ctpsSerial);
            suframaCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.suframaCode);
            monthlyIncome = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.monthlyIncome);
            branchInsertCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.branchInsertCode);
            codeMainRelated = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.codeMainRelated);
            shippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.shippingCompanyCode);
            bridgeShippingCompanyCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.bridgeShippingCompanyCode);
            maximumAverageTerm = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.maximumAverageTerm);
            priceTableCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.priceTableCode);
            orderPriority = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.orderPriority);
            bearerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.bearerCode);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(customerDto.preferences?.paymentConditionCode);

            birthDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(customerDto.birthDate);
            hireDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(customerDto.hireDate);
            insertDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(customerDto.insertDate);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(customerDto.maxChangeFilterDate);
            
            isInactive = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isInactive);
            isCustomer = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isCustomer);
            isEmployee = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isCustomer);
            isSupplier = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isSupplier);
            isRepresentative = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isRepresentative);
            isPurchasingGuide = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isPurchasingGuide);
            isShippingCompany = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerDto.isShippingCompany);

            percentegeValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(customerDto.shippingCompany?.percentegeValue);
            kilogramsValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(customerDto.shippingCompany?.kilogramsValue);
            minimunValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(customerDto.shippingCompany?.minimunValue);
            maximumValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(customerDto.shippingCompany?.maximumValue);
            
            cpf = customerDto.cpf?.Length > 14 ? customerDto.cpf.Substring(0, 14) : customerDto.cpf;
            maritalStatus = customerDto.maritalStatus?.Length > 60 ? customerDto.maritalStatus.Substring(0, 60) : customerDto.maritalStatus;
            gender = customerDto.gender?.Length > 60 ? customerDto.gender.Substring(0, 60) : customerDto.gender;
            rg = customerDto.rg?.Length > 11 ? customerDto.rg.Substring(0, 11) : customerDto.rg;
            rgFederalAgency = customerDto.rgFederalAgency?.Length > 60 ? customerDto.rgFederalAgency.Substring(0, 60) : customerDto.rgFederalAgency;
            motherName = customerDto.motherName?.Length > 250 ? customerDto.motherName.Substring(0, 250) : customerDto.motherName;
            fatherName = customerDto.fatherName?.Length > 250 ? customerDto.fatherName.Substring(0, 250) : customerDto.fatherName;
            nationality = customerDto.nationality?.Length > 60 ? customerDto.nationality.Substring(0, 60) : customerDto.nationality;
            occupation = customerDto.occupation?.Length > 60 ? customerDto.occupation.Substring(0, 60) : customerDto.occupation;
            workPlace = customerDto.workPlace?.Length > 60 ? customerDto.workPlace.Substring(0, 60) : customerDto.workPlace;
            homeTown = customerDto.homeTown?.Length > 60 ? customerDto.homeTown.Substring(0, 60) : customerDto.homeTown;
            name = customerDto.name?.Length > 250 ? customerDto.name.Substring(0, 250) : customerDto.name;
            homePage = customerDto.homePage?.Length > 300 ? customerDto.homePage.Substring(0, 300) : customerDto.homePage;
            cpfCnpjMainRelated = customerDto.cpfCnpjMainRelated?.Length > 14 ? customerDto.cpfCnpjMainRelated.Substring(0, 14) : customerDto.cpfCnpjMainRelated;
            nameMainRelated = customerDto.nameMainRelated?.Length > 60 ? customerDto.nameMainRelated.Substring(0, 60) : customerDto.nameMainRelated;
            employeeStatus = customerDto.employeeStatus?.Length > 60 ? customerDto.employeeStatus.Substring(0, 60) : customerDto.employeeStatus;
            customerStatus = customerDto.customerStatus?.Length > 60 ? customerDto.customerStatus.Substring(0, 60) : customerDto.customerStatus;
            paymentConditionDescription = customerDto.preferences?.paymentConditionDescription?.Length > 60 ? customerDto.preferences?.paymentConditionDescription.Substring(0, 60) : customerDto.preferences?.paymentConditionDescription;
            shippingCompanyName = customerDto.preferences?.shippingCompanyName?.Length > 250 ? customerDto.preferences?.shippingCompanyName.Substring(0, 250) : customerDto.preferences?.shippingCompanyName;
            freightType = customerDto.preferences?.freightType?.Length > 60 ? customerDto.preferences?.freightType.Substring(0, 60) : customerDto.preferences?.freightType;
            bridgeShippingCompanyName = customerDto.preferences?.bridgeShippingCompanyName?.Length > 60 ? customerDto.preferences?.bridgeShippingCompanyName.Substring(0, 60) : customerDto.preferences?.bridgeShippingCompanyName;
            priceTableDescription = customerDto.preferences?.priceTableDescription?.Length > 250 ? customerDto.preferences?.priceTableDescription.Substring(0, 250) : customerDto.preferences?.priceTableDescription;
            anttCode = customerDto.shippingCompany?.anttCode?.Length > 60 ? customerDto.shippingCompany?.anttCode.Substring(0, 60) : customerDto.shippingCompany?.anttCode;
            typeDeductIcms = customerDto.shippingCompany?.typeDeductIcms?.Length > 60 ? customerDto.shippingCompany?.typeDeductIcms.Substring(0, 60) : customerDto.shippingCompany?.typeDeductIcms;
            bearerName = customerDto.preferences?.bearerName?.Length > 60 ? customerDto.preferences?.bearerName.Substring(0, 60) : customerDto.preferences?.bearerName;

            Responses.Add(customerDto.code, JsonConvert.SerializeObject(customerDto));

            customerDto.addresses?.ForEach(address =>
                addresses.Add(new Address(address, code)
            ));

            customerDto.phones?.ForEach(phone =>
                phones.Add(new Phone(phone, code)
            ));

            customerDto.emails?.ForEach(email =>
                emails.Add(new Email(email, code)
            ));

            customerDto.observations?.ForEach(observation =>
                observations.Add(new Observation(observation, code)
            ));
            
            customerDto.familiars?.ForEach(familiar =>
                familiars.Add(new Familiar(familiar, code)
            ));

            customerDto.customerObservations?.ForEach(customerObservation =>
                customerObservations.Add(new CustomerObservation(customerObservation, code)
            ));

            customerDto.additionalFields?.ForEach(additionalField =>
                additionalFields.Add(new AdditionalField(additionalField, code)
            ));

            customerDto.classifications?.ForEach(classification =>
                classifications.Add(new Classification(classification, code)
            ));

            customerDto.references?.ForEach(reference =>
                references.Add(new Reference(reference, code)
            ));

            customerDto.relateds?.ForEach(related =>
                relateds.Add(new Related(related, code)
            ));

            customerDto.statistics?.ForEach(statistic =>
                statistics.Add(new Statistic(statistic, code)
            ));
            customerDto.representatives?.ForEach(representative =>
                representatives.Add(new Representative(representative, code)
            ));
        }
    }
}
