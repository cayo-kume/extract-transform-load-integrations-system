namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// corpo do POST person/v2/individual-customers (IndividualCustomerInDto): cria ou altera um cliente pessoa física pelo cpf. Campo
    /// ausente não muda; para zerar, "" (texto e enum), 0 (número) ou "NotInformed" (enum). Tudo é anulável e os nulos não vão, inclusive o
    /// que o swagger declara sem nullable (insertDate). Os enums vão pelo nome (CustomerEnumNames, PE6)
    /// </summary>
    public class IndividualCustomer
    {
        public int? branchInsertCode { get; init; }
        public DateTime? insertDate { get; init; }
        public string? name { get; init; }
        public string? cpf { get; init; }
        public string? rg { get; init; }
        public string? rgFederalAgency { get; init; }
        public string? birthDate { get; init; }
        public string? gender { get; init; }
        public bool? isInactive { get; init; }
        public string? maritalStatus { get; init; }
        public string? motherName { get; init; }
        public string? fatherName { get; init; }
        public string? nationality { get; init; }
        public string? homeTown { get; init; }
        public decimal? monthlyIncome { get; init; }
        public string? occupation { get; init; }
        public string? hireDate { get; init; }
        public string? workPlace { get; init; }
        public long? ctps { get; init; }
        public long? ctpsSerial { get; init; }
        public string? homePage { get; init; }
        public int? employee { get; init; }
        public bool? employeeIsInactive { get; init; }
        public string? registrationStatus { get; init; }
        public bool? isBloqued { get; init; }
        public bool? isFinalCNSR { get; init; }
        public int? suframaCode { get; init; }
        public string? taxCodeNumber { get; init; }
        public List<PhoneIn>? phones { get; init; }
        public List<AddressIn>? addresses { get; init; }
        public List<EmailIn>? emails { get; init; }
        public List<ReferenceIn>? references { get; init; }
        public List<ObservationIn>? observations { get; init; }
        public List<AdditionalFieldIn>? additionalFields { get; init; }
        public List<ClassificationIn>? classifications { get; init; }
        public List<LimitIn>? limits { get; init; }
        public List<SocialNetworkIn>? socialNetworks { get; init; }
        public IndividualAdditional? individualAdditionals { get; init; }
        public List<BankAccountIn>? bankAccounts { get; init; }
    }
}
