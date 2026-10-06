using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class Person
    {
        public string id { get; private set; }
        public string codeReferenceAdditional { get; private set; }
        public string accessProfile { get; private set; }
        public string corporateName { get; private set; }
        public string businessName { get; private set; }
        public string businessBranch { get; private set; }
        public string cpfCnpj { get; private set; }
        public string userName { get; private set; }
        public string password { get; private set; }
        public string role { get; private set; }
        public string bossId { get; private set; }
        public string bossName { get; private set; }
        public string classification { get; private set; }
        public string cultureId { get; private set; }
        public string timeZoneId { get; private set; }
        public string authenticateOn { get; private set; }
        public string createdBy { get; private set; }
        public string changedBy { get; private set; }
        public string observations { get; private set; }

        public bool isActive { get; private set; }

        public int personType { get; private set; }
        public int profileType { get; private set; }

        public DateTime createdDate { get; private set; }
        public DateTime changedDate { get; private set; }

        [SkipProperty]
        public List<Team> teams { get; private set; } = [];
        
        [SkipProperty]
        public List<Address> addresses { get; private set; } = [];

        [SkipProperty]
        public List<Contact> contacts { get; private set; } = [];

        [SkipProperty]
        public List<Email> emails { get; private set; } = [];

        [SkipProperty]
        public List<Relationship> relationships { get; private set; } = [];

        [SkipProperty]
        public List<CustomFieldValue> customFieldValues { get; private set; } = [];

        [SkipProperty]
        public List<Asset> atAssets { get; private set; } = [];

        public Person(Dtos.Person.Person personDto)
        {
            id = personDto.id;
            codeReferenceAdditional = personDto.codeReferenceAdditional;
            accessProfile = personDto.accessProfile;
            corporateName = personDto.corporateName;
            businessName = personDto.businessName;
            businessBranch = personDto.businessBranch;
            cpfCnpj = personDto.cpfCnpj;
            userName = personDto.userName;
            password = personDto.password;
            role = personDto.role;
            bossId = personDto.bossId;
            bossName = personDto.bossName;
            classification = personDto.classification;
            cultureId = personDto.cultureId;
            timeZoneId = personDto.timeZoneId;
            authenticateOn = personDto.authenticateOn;
            createdBy = personDto.createdBy;
            changedBy = personDto.changedBy;
            observations = personDto.observations;

            isActive = ConverterValidatorsExtensions.ConvertToBooleanValidation(personDto.isActive);

            personType = ConverterValidatorsExtensions.ConvertToInt32Validation(personDto.personType);
            profileType = ConverterValidatorsExtensions.ConvertToInt32Validation(personDto.profileType);

            createdDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(personDto.createdDate);
            changedDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(personDto.changedDate);

            for (int i = 0; i <= personDto.teams?.Count(); i++)
            {
                teams.Add(new Team(personDto.id, i + 1, personDto.teams[i]));
            }

            personDto.addresses?.ForEach(address =>
                addresses.Add(new Address(personDto.id, address)
            ));

            personDto.contacts?.ForEach(contact =>
                contacts.Add(new Contact(personDto.id, contact)
            ));

            personDto.emails?.ForEach(email =>
                emails.Add(new Email(personDto.id, email)
            ));

            personDto.relationships?.ForEach(relationship =>
                relationships.Add(new Relationship(personDto.id, relationship)
            ));

            personDto.customFieldValues?.ForEach(customFieldValue =>
                customFieldValues.Add(new CustomFieldValue(personDto.id, customFieldValue)
            ));

            personDto.atAssets?.ForEach(atAsset =>
                atAssets.Add(new Asset(personDto.id, atAsset)
            ));
        }
    }
}
