namespace Domain.Movidesk.Dtos.Reads.People
{
    public class Person
    {
        public string id { get; init; }
        public string codeReferenceAdditional { get; init; }
        public string isActive { get; init; }
        public string personType { get; init; }
        public string profileType { get; init; }
        public string accessProfile { get; init; }
        public string corporateName { get; init; }
        public string businessName { get; init; }
        public string businessBranch { get; init; }
        public string cpfCnpj { get; init; }
        public string userName { get; init; }
        public string role { get; init; }
        public string bossId { get; init; }
        public string bossName { get; init; }
        public string classification { get; init; }
        public string cultureId { get; init; }
        public string timeZoneId { get; init; }
        public string authenticateOn { get; init; }
        public string createdDate { get; init; }
        public string createdBy { get; init; }
        public string changedDate { get; init; }
        public string changedBy { get; init; }
        public string observations { get; init; }
        public List<Address>? addresses { get; init; }
        public List<Contact>? contacts { get; init; }
        public List<Email>? emails { get; init; }
        public List<string>? teams { get; init; }
        public List<Relationship>? relationships { get; init; }
        public List<CustomFieldValue>? customFieldValues { get; init; }
        public List<Asset>? atAssets { get; init; }
    }
}
