using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class ActionTag
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int actionId { get; private set; }
        public string tag { get; private set; }

        public ActionTag() { }

        public ActionTag(string doc_company, DateTime integrated_or_updated_at, long ticketId, int actionId, string tag)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.actionId = actionId;
            this.tag = ConverterValidatorsExtensions.StringLengthValidation(tag, 128);
        }
    }
}
