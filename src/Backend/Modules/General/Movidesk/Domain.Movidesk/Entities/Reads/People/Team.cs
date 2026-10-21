using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class Team
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public string team { get; private set; }

        public Team() { }

        public Team(string doc_company, DateTime integrated_or_updated_at, string personId, string team)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.team = ConverterValidatorsExtensions.StringLengthValidation(team, 128);
        }
    }
}
