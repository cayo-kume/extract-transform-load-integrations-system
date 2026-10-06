using Domain.Core.Extensions;
using System.Xml.Linq;

namespace Domain.Movidesk.Entities.Person
{
    public class Relationship
    {
        public string personId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string slaAgreement { get; private set; }
        public bool forceChildrenToHaveSomeAgreement { get; private set; }
        public bool allowAllServices { get; private set; }
        public bool includeInParents { get; private set; }
        public bool loadChildOrganizations { get; private set; }
        public bool isGetMethod { get; private set; }

        [SkipProperty]
        public List<Service> services { get; private set; } = [];

        public Relationship() { }

        public Relationship(string personId, Dtos.Person.Relationship relationshipDto)
        {
            this.personId = personId;
            id = relationshipDto.id;
            name = relationshipDto.name;
            slaAgreement = relationshipDto.slaAgreement;

            forceChildrenToHaveSomeAgreement = ConverterValidatorsExtensions.ConvertToBooleanValidation(relationshipDto.forceChildrenToHaveSomeAgreement);
            allowAllServices = ConverterValidatorsExtensions.ConvertToBooleanValidation(relationshipDto.allowAllServices);
            includeInParents = ConverterValidatorsExtensions.ConvertToBooleanValidation(relationshipDto.includeInParents);
            loadChildOrganizations = ConverterValidatorsExtensions.ConvertToBooleanValidation(relationshipDto.loadChildOrganizations);
            isGetMethod = ConverterValidatorsExtensions.ConvertToBooleanValidation(relationshipDto.isGetMethod);

            relationshipDto.services?.ForEach(service =>
                services.Add(new Service(personId, relationshipDto.id, service)
            ));
        }
    }
}
