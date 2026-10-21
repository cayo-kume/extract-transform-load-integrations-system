using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Asset
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string label { get; private set; }

        public Asset() { }

        public Asset(string doc_company, DateTime integrated_or_updated_at, string personId, Dtos.Reads.People.Asset dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;

            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 64);
            label = ConverterValidatorsExtensions.StringLengthValidation(dto.label, 64);
        }
    }
}
