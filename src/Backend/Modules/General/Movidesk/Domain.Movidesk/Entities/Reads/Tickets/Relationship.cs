using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    /// <summary>
    /// parentTickets e childrenTickets numa tabela só (MovideskTicketsRelationships), diferenciados por relationType
    /// </summary>
    public class Relationship
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public long relatedTicketId { get; private set; }
        public string relationType { get; private set; }
        public string subject { get; private set; }
        public bool isDeleted { get; private set; }

        public Relationship() { }

        public Relationship(string doc_company, DateTime integrated_or_updated_at, long ticketId, string relationType, Dtos.Reads.Tickets.ParentChildTicket dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.relationType = relationType;

            relatedTicketId = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
            subject = ConverterValidatorsExtensions.StringLengthValidation(dto.subject, 350);
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
        }
    }
}
