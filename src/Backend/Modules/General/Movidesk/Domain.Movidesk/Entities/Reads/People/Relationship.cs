using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Relationship
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string slaAgreement { get; private set; }
        public bool forceChildrenToHaveSomeAgreement { get; private set; }
        public bool? allowAllServices { get; private set; }
        public bool? includeInParents { get; private set; }
        public bool? loadChildOrganizations { get; private set; }

        [SkipProperty]
        public List<Service> services { get; private set; } = [];

        public Relationship() { }

        public Relationship(string doc_company, DateTime integrated_or_updated_at, string personId, Dtos.Reads.People.Relationship dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;

            // relacionamento sem organização: a API manda id null; StringLengthValidation devolve '' porque a coluna faz parte da PK
            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            slaAgreement = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreement, 128);
            forceChildrenToHaveSomeAgreement = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.forceChildrenToHaveSomeAgreement);
            allowAllServices = NullableConverters.ToNullableBoolean(dto.allowAllServices);
            includeInParents = NullableConverters.ToNullableBoolean(dto.includeInParents);
            loadChildOrganizations = NullableConverters.ToNullableBoolean(dto.loadChildOrganizations);

            foreach (var service in (dto.services ?? []).Where(s => !String.IsNullOrEmpty(s.id)))
                services.Add(new Service(doc_company, integrated_or_updated_at, personId, id, service));
        }
    }
}
