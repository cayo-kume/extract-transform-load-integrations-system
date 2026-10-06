using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class Service
    {
        public string personId { get; private set; }
        public string relationshipId { get; private set; }
        public int id { get; private set; }
        public string name { get; private set; }
        public bool copyToChildren { get; private set; }

        public Service() { }

        public Service(string personId, string relationshipId, Dtos.Person.Service serviceDto)
        {
            this.personId = personId;
            this.relationshipId = relationshipId;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(serviceDto.id);

            name = serviceDto.name;

            copyToChildren = ConverterValidatorsExtensions.ConvertToBooleanValidation(serviceDto.copyToChildren);
        }
    }
}
