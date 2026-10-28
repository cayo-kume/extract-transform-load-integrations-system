namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// valores aceitos nos enums das escritas de cliente: o swagger da API Person declara esses enums como string e a descrição zera um campo
    /// com "NotInformed" ou "" (PE6), então os DTOs levam o nome como texto e o validador confere contra estas listas (grafia do swagger)
    /// </summary>
    public static class CustomerEnumNames
    {
        public static readonly IReadOnlyList<string> Gender = ["NotInformed", "Female", "Male"];
        public static readonly IReadOnlyList<string> MaritalStatus = ["NotInformed", "Single", "Married", "Widowed", "Cohabitating", "Separated", "Divorced", "NotDeclared"];
        public static readonly IReadOnlyList<string> RegistrationStatus = ["Normal", "InAnalysis"];
        public static readonly IReadOnlyList<string> Address = ["Commercial", "Residential", "Billing", "Shopping", "Delivery", "Mailing", "ShowRoom", "PickUp", "CommercialPrevious", "ResidentialPrevious"];
        public static readonly IReadOnlyList<string> Reference = ["Banking", "Commercial", "Others", "Personal", "ConsSci"];
        public static readonly IReadOnlyList<string> EducationLevel = ["Illiterate", "ElementaryIncomplete", "ElementaryComplete", "HighSchoolIncomplete", "HighSchoolComplete", "UndergraduateIncomplete", "UndergraduateComplete", "PostGraduate", "Masters", "Doctorate", "Postdoctoral"];
        public static readonly IReadOnlyList<string> Home = ["Owned", "Rented", "Ceded", "Others", "Parentes", "Financed"];
        public static readonly IReadOnlyList<string> Car = ["DontHave", "Financed", "PaidOff"];
        public static readonly IReadOnlyList<string> DriverLicenseCategory = ["A", "B", "C", "D", "E", "AB", "AD", "AE"];
        public static readonly IReadOnlyList<string> MilitaryCertificate = ["NotSubject", "FirstCategory", "SecondCategory", "ThirdCategory", "ReserveOfficer", "OtherCategories", "MilitaryEnlistment", "NotEnlisted"];
        public static readonly IReadOnlyList<string> MilitaryCategory = ["FirstCategory", "SecondCategory", "ThirdCategory"];
        public static readonly IReadOnlyList<string> Person = ["NotInformed", "Individual", "Legal"];
        public static readonly IReadOnlyList<string> BankAccount = ["CurrentAccount", "SavingsAccount", "SalaryAccount", "PaymentAccount", "JointCurrentAccount", "JointSavingsAccount", "JointJudicialDepositAccount"];
        public static readonly IReadOnlyList<string> BankAccountPurpose = ["ElectronicScheduling", "SalaryScheduling", "PIXTransfer"];
        public static readonly IReadOnlyList<string> BankAccountCheck = ["Common", "banking", "salary", "administrative", "CPMF"];
        public static readonly IReadOnlyList<string> PixKey = ["Phone", "Email", "CPF_CNPJ", "Random"];
        public static readonly IReadOnlyList<string> State = ["NotInformed", "DF", "ES", "RR", "GO", "PE", "PI", "RJ", "RN", "RS", "RO", "TO", "SC", "SP", "SE", "AM", "BA", "CE", "AL", "AC", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "AP", "EX"];
        public static readonly IReadOnlyList<string> NumberEmployer = ["NotInformed", "Until10", "From11to50", "From51to100", "From101to500", "From501to1000", "GreaterThan1000"];
        public static readonly IReadOnlyList<string> TaxRegime = ["NotInformed", "Normal", "MicroBranch", "Epp", "RealProfit", "PresumedProfit", "Mei", "Eireli", "Others"];
        public static readonly IReadOnlyList<string> SubTributary = ["NotInformed", "Calculate", "NotCalculate"];
        public static readonly IReadOnlyList<string> Freight = ["NotInformed", "CIF", "FOB", "ThirdParty", "NoOccurrence", "OwnSender", "OwnRecipient"];
    }
}
