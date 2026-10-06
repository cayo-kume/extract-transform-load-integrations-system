namespace Domain.Totvs.Dtos.Person.Customer
{
    public class Item
    {
        public string code { get; set; }
        public string cpf { get; set; }
        public string isInactive { get; set; }
        public string maritalStatus { get; set; }
        public string gender { get; set; }
        public string ctps { get; set; }
        public string ctpsSerial { get; set; }
        public string rg { get; set; }
        public string rgFederalAgency { get; set; }
        public string suframaCode { get; set; }
        public string motherName { get; set; }
        public string fatherName { get; set; }
        public string nationality { get; set; }
        public string birthDate { get; set; }
        public string monthlyIncome { get; set; }
        public string occupation { get; set; }
        public string hireDate { get; set; }
        public string workPlace { get; set; }
        public string homeTown { get; set; }
        public string branchInsertCode { get; set; }
        public string name { get; set; }
        public string homePage { get; set; }
        public string codeMainRelated { get; set; }
        public string cpfCnpjMainRelated { get; set; }
        public string nameMainRelated { get; set; }
        public string insertDate { get; set; }
        public string maxChangeFilterDate { get; set; }
        public string isCustomer { get; set; }
        public string isSupplier { get; set; }
        public string isRepresentative { get; set; }
        public string isPurchasingGuide { get; set; }
        public string isShippingCompany { get; set; }
        public string isEmployee { get; set; }
        public string employeeStatus { get; set; }
        public string customerStatus { get; set; }

        public List<Address> addresses { get; set; }
        public List<Phone> phones { get; set; }
        public List<Email> emails { get; set; }
        public List<Observation> observations { get; set; }
        public List<Customerobservation> customerObservations { get; set; }
        public List<Additionalfield> additionalFields { get; set; }
        public List<Classification> classifications { get; set; }
        public List<Reference> references { get; set; }
        public List<Related> relateds { get; set; }
        public Shippingcompany shippingCompany { get; set; }
        public List<Statistic> statistics { get; set; }
        public List<Representative> representatives { get; set; }
        public Preferences preferences { get; set; }
        public List<Familiar> familiars { get; set; }
    }
}
