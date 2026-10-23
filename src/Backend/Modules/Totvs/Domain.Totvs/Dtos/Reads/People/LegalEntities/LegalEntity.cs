using Domain.Totvs.Dtos.Reads.People.Persons;

namespace Domain.Totvs.Dtos.Reads.People.LegalEntities
{
    /// <summary>
    /// pessoa jurídica do POST person/v2/legal-entities/search (LegalEntityDataModel)
    /// </summary>
    public class LegalEntity
    {
        public string? code { get; init; }
        public string? cnpj { get; init; }
        public string? isInactive { get; init; }
        public string? branchInsertCode { get; init; }
        public string? name { get; init; }
        public string? fantasyName { get; init; }
        public string? uf { get; init; }
        public string? numberStateRegistration { get; init; }
        public string? suframaCode { get; init; }
        public string? dateFoundation { get; init; }
        public string? codeActivity { get; init; }
        public string? numberEmployees { get; init; }
        public string? monthlyInvoicing { get; init; }
        public string? shareCapital { get; init; }
        public string? homePage { get; init; }
        public string? codeActivityCnae { get; init; }
        public string? codeActivityCnae2 { get; init; }
        public string? typeTaxRegime { get; init; }
        public string? typeSubTributary { get; init; }
        public string? typeDescriptionSuFrama { get; init; }
        public string? registrationMunicipal { get; init; }
        public string? descriptionJuntaCial { get; init; }
        public string? dateRegJuntaCial { get; init; }
        public string? codeMainRelated { get; init; }
        public string? cpfCnpjMainRelated { get; init; }
        public string? nameMainRelated { get; init; }
        public string? insertDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? isCustomer { get; init; }
        public string? isSupplier { get; init; }
        public string? isRepresentative { get; init; }
        public string? isPurchasingGuide { get; init; }
        public string? isShippingCompany { get; init; }
        public string? customerStatus { get; init; }
        public string? status { get; init; }

        public ShippingCompany? shippingCompany { get; init; }
        public Preferences? preferences { get; init; }

        public List<Address>? addresses { get; init; }
        public List<Phone>? phones { get; init; }
        public List<Email>? emails { get; init; }
        public List<Observation>? observations { get; init; }
        public List<CustomerObservation>? customerObservations { get; init; }
        public List<AdditionalField>? additionalFields { get; init; }
        public List<Classification>? classifications { get; init; }
        public List<Reference>? references { get; init; }
        public List<Related>? relateds { get; init; }
        public List<Partner>? partners { get; init; }
        public List<Contact>? contacts { get; init; }
        public List<Statistic>? statistics { get; init; }
        public List<SocialNetwork>? socialNetworks { get; init; }
        public List<Representative>? representatives { get; init; }
        public List<string>? paymentMethods { get; init; }
    }
}
