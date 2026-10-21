using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Client
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public string clientId { get; private set; }
        public string? organizationId { get; private set; }
        public bool isDeleted { get; private set; }

        public Client() { }

        public Client(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.Client dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;

            clientId = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            organizationId = dto.organization?.id;
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
        }
    }
}
