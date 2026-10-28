namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// corpo do POST person/v2/legal-customers (LegalEntityCustomerInDto): cria ou altera um cliente pessoa jurídica pelo cnpj, com as
    /// mesmas regras do IndividualCustomer (isCommercialExp, sem nullable no swagger, fica de fora quando nulo)
    /// </summary>
    public class LegalCustomer
    {
        public int? branchInsertCode { get; init; }
        public DateTime? insertDate { get; init; }
        public string? cnpj { get; init; }
        public string? name { get; init; }
        public string? fantasyName { get; init; }
        public bool? isInactive { get; init; }
        public string? uf { get; init; }
        public string? numberStateRegistration { get; init; }
        public string? dateFoundation { get; init; }
        public int? codeActivity { get; init; }
        public string? numberEmployees { get; init; }
        public decimal? monthlyInvoicing { get; init; }
        public decimal? shareCapital { get; init; }
        public string? homePage { get; init; }
        public string? registrationStatus { get; init; }
        public int? codeActivityCnae { get; init; }
        public int? codeActivityCnae2 { get; init; }
        public string? typeTaxRegime { get; init; }
        public string? typeSubTributary { get; init; }
        public int? suframaCode { get; init; }
        public string? registrationMunicipal { get; init; }
        public string? descriptionJuntaCial { get; init; }
        public string? dateRegJuntaCial { get; init; }
        public string? foreignIdentification { get; init; }
        public string? initialDateActivity { get; init; }
        public string? finalDateActivity { get; init; }
        public bool? isCommercialExp { get; init; }
        public bool? isFinalCNSR { get; init; }
        public List<PhoneIn>? phones { get; init; }
        public List<AddressIn>? addresses { get; init; }
        public List<EmailIn>? emails { get; init; }
        public List<PartnerIn>? partners { get; init; }
        public List<ObservationIn>? observations { get; init; }
        public List<AdditionalFieldIn>? additionalFields { get; init; }
        public List<ClassificationIn>? classifications { get; init; }
        public List<ReferenceIn>? references { get; init; }
        public List<SocialNetworkIn>? socialNetworks { get; init; }
        public List<ContactIn>? contacts { get; init; }
        public List<LimitIn>? limits { get; init; }
        public Preferences? preferences { get; init; }
    }
}
