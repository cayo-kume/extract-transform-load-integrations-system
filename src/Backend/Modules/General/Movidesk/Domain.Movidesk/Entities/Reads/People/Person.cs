using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Person
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string id { get; private set; }
        public string codeReferenceAdditional { get; private set; }
        public bool isActive { get; private set; }
        public int? personType { get; private set; }
        public int? profileType { get; private set; }
        public string accessProfile { get; private set; }
        public string corporateName { get; private set; }
        public string businessName { get; private set; }
        public string businessBranch { get; private set; }
        public string cpfCnpj { get; private set; }
        public string userName { get; private set; }
        public string role { get; private set; }
        public string? bossId { get; private set; }
        public string bossName { get; private set; }
        public string classification { get; private set; }
        public string cultureId { get; private set; }
        public string timeZoneId { get; private set; }
        public string? authenticateOn { get; private set; }
        public DateTime? createdDate { get; private set; }
        public string? createdById { get; private set; }
        public DateTime? changedDate { get; private set; }
        public string? changedById { get; private set; }
        public string? observations { get; private set; }

        [SkipProperty]
        public List<Address> addresses { get; private set; } = [];

        [SkipProperty]
        public List<Contact> contacts { get; private set; } = [];

        [SkipProperty]
        public List<Email> emails { get; private set; } = [];

        [SkipProperty]
        public List<Team> teams { get; private set; } = [];

        [SkipProperty]
        public List<Relationship> relationships { get; private set; } = [];

        [SkipProperty]
        public List<CustomFieldValue> customFieldValues { get; private set; } = [];

        [SkipProperty]
        public List<Asset> atAssets { get; private set; } = [];

        public Person() { }

        public Person(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Person dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            codeReferenceAdditional = ConverterValidatorsExtensions.StringLengthValidation(dto.codeReferenceAdditional, 64);
            isActive = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isActive);
            personType = NullableConverters.ToNullableInt32(dto.personType);
            profileType = NullableConverters.ToNullableInt32(dto.profileType);
            accessProfile = ConverterValidatorsExtensions.StringLengthValidation(dto.accessProfile, 128);
            corporateName = ConverterValidatorsExtensions.StringLengthValidation(dto.corporateName, 128);
            businessName = ConverterValidatorsExtensions.StringLengthValidation(dto.businessName, 128);
            businessBranch = ConverterValidatorsExtensions.StringLengthValidation(dto.businessBranch, 128);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            userName = ConverterValidatorsExtensions.StringLengthValidation(dto.userName, 64);
            role = ConverterValidatorsExtensions.StringLengthValidation(dto.role, 128);
            bossId = dto.bossId;
            bossName = ConverterValidatorsExtensions.StringLengthValidation(dto.bossName, 128);
            classification = ConverterValidatorsExtensions.StringLengthValidation(dto.classification, 128);
            cultureId = ConverterValidatorsExtensions.StringLengthValidation(dto.cultureId, 32);
            timeZoneId = ConverterValidatorsExtensions.StringLengthValidation(dto.timeZoneId, 64);
            authenticateOn = dto.authenticateOn;   // nvarchar(max)
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
            createdById = dto.createdBy;
            changedDate = NullableConverters.ToNullableDateTime(dto.changedDate);   // nulo para quem nunca foi alterado
            changedById = dto.changedBy;
            observations = dto.observations;       // nvarchar(max)

            // endereços, contatos e e-mails não têm chave na API: posição na lista (a lista inteira é regravada a cada lote)
            var dtoAddresses = dto.addresses ?? [];
            for (int i = 0; i < dtoAddresses.Count; i++)
                addresses.Add(new Address(doc_company, integrated_or_updated_at, id, i + 1, dtoAddresses[i]));

            var dtoContacts = dto.contacts ?? [];
            for (int i = 0; i < dtoContacts.Count; i++)
                contacts.Add(new Contact(doc_company, integrated_or_updated_at, id, i + 1, dtoContacts[i]));

            var dtoEmails = dto.emails ?? [];
            for (int i = 0; i < dtoEmails.Count; i++)
                emails.Add(new Email(doc_company, integrated_or_updated_at, id, i + 1, dtoEmails[i]));

            // equipes repetidas violariam a PK (o SQL Server não diferencia maiúsculas de minúsculas)
            foreach (var team in (dto.teams ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
                teams.Add(new Team(doc_company, integrated_or_updated_at, id, team));

            // um único relacionamento por organização; o sem organização (id null -> '') também só pode haver um
            foreach (var relationship in (dto.relationships ?? []).GroupBy(r => r.id ?? String.Empty).Select(g => g.First()))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, relationship));

            foreach (var customFieldValue in dto.customFieldValues ?? [])
                customFieldValues.Add(new CustomFieldValue(doc_company, integrated_or_updated_at, id, customFieldValue));

            foreach (var asset in (dto.atAssets ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
                atAssets.Add(new Asset(doc_company, integrated_or_updated_at, id, asset));
        }
    }
}
