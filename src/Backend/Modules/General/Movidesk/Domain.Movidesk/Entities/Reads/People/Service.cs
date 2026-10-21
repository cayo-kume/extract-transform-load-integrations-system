using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    /// <summary>
    /// serviço que a pessoa pode usar numa organização (relationships[].services[])
    /// </summary>
    public class Service
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string relationshipId { get; private set; }
        public int id { get; private set; }
        public string name { get; private set; }
        public bool? copyToChildren { get; private set; }

        public Service() { }

        public Service(string doc_company, DateTime integrated_or_updated_at, string personId, string relationshipId, Dtos.Reads.People.Service dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.relationshipId = relationshipId;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.id);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            copyToChildren = NullableConverters.ToNullableBoolean(dto.copyToChildren);
        }
    }
}
