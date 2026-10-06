namespace Domain.Totvs.Dtos.Person.LegalEntity
{
    public class Item
    {
        public string? code { get; set; }
        public string cnpj { get; set; }
        public string? isInactive { get; set; }
        public string? branchInsertCode { get; set; }
        public string? name { get; set; }
        public string? fantasyName { get; set; }
        public string? uf { get; set; }
        public string? numberStateRegistration { get; set; }
        public string? suframaCode { get; set; }
        public string? dateFoundation { get; set; }
        public string? codeActivity { get; set; }
        public string? numberEmployees { get; set; }
        public string? monthlyInvoicing { get; set; }
        public string? shareCapital { get; set; }
        public string? homePage { get; set; }
        public string? codeActivityCnae { get; set; }
        public string? codeActivityCnae2 { get; set; }
        public string? typeTaxRegime { get; set; }
        public string? typeSubTributary { get; set; }
        public string? typeDescriptionSuFrama { get; set; }
        public string? registrationMunicipal { get; set; }
        public string? descriptionJuntaCial { get; set; }
        public string? dateRegJuntaCial { get; set; }
        public string? codeMainRelated { get; set; }
        public string? cpfCnpjMainRelated { get; set; }
        public string? nameMainRelated { get; set; }
        public string? insertDate { get; set; }
        public string? maxChangeFilterDate { get; set; }
        public string? isCustomer { get; set; }
        public string? isSupplier { get; set; }
        public string? isRepresentative { get; set; }
        public string? isPurchasingGuide { get; set; }
        public string? isShippingCompany { get; set; }
        public string? customerStatus { get; set; }
        public string status { get; set; }
        
        public Preferences? preferences { get; set; }
        public Shippingcompany? shippingCompany { get; set; }
        
        public List<int>? paymentMethods { get; set; }
        
        public List<Address>? addresses { get; set; }
        public List<Phone>? phones { get; set; }
        public List<Email>? emails { get; set; }
        public List<Observation>? observations { get; set; }
        public List<Customerobservation>? customerObservations { get; set; }
        public List<Additionalfield>? additionalFields { get; set; }
        public List<Classification>? classifications { get; set; }
        public List<Reference>? references { get; set; }
        public List<Related>? relateds { get; set; }
        public List<Partner>? partners { get; set; }
        public List<Contact>? contacts { get; set; }
        public List<Statistic>? statistics { get; set; }
        public List<Socialnetwork>? socialNetworks { get; set; }
        public List<Representative>? representatives { get; set; }
    }
}
