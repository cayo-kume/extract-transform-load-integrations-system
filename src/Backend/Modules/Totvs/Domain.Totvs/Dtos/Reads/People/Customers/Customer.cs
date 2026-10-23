using Domain.Totvs.Dtos.Reads.People.Persons;

namespace Domain.Totvs.Dtos.Reads.People.Customers
{
    /// <summary>
    /// pessoa física do POST person/v2/individuals/search (IndividualDataModel)
    /// </summary>
    public class Customer
    {
        public string? code { get; init; }
        public string? cpf { get; init; }
        public string? isInactive { get; init; }
        public string? maritalStatus { get; init; }
        public string? gender { get; init; }
        public string? ctps { get; init; }
        public string? ctpsSerial { get; init; }
        public string? rg { get; init; }
        public string? rgFederalAgency { get; init; }
        public string? suframaCode { get; init; }
        public string? motherName { get; init; }
        public string? fatherName { get; init; }
        public string? nationality { get; init; }
        public string? birthDate { get; init; }
        public string? monthlyIncome { get; init; }
        public string? occupation { get; init; }
        public string? hireDate { get; init; }
        public string? workPlace { get; init; }
        public string? homeTown { get; init; }
        public string? branchInsertCode { get; init; }
        public string? name { get; init; }
        public string? homePage { get; init; }
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
        public string? isEmployee { get; init; }
        public string? employeeStatus { get; init; }
        public string? customerStatus { get; init; }

        public ShippingCompany? shippingCompany { get; init; }
        public Preferences? preferences { get; init; }
        public IndividualAdditionals? individualAdditionals { get; init; }

        public List<Address>? addresses { get; init; }
        public List<Phone>? phones { get; init; }
        public List<Email>? emails { get; init; }
        public List<Observation>? observations { get; init; }
        public List<CustomerObservation>? customerObservations { get; init; }
        public List<AdditionalField>? additionalFields { get; init; }
        public List<Classification>? classifications { get; init; }
        public List<Reference>? references { get; init; }
        public List<Related>? relateds { get; init; }
        public List<Statistic>? statistics { get; init; }
        public List<Representative>? representatives { get; init; }
        public List<Familiar>? familiars { get; init; }
    }
}
