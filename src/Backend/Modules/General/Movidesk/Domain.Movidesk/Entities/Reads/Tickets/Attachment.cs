using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Attachment
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int actionId { get; private set; }
        public string path { get; private set; }
        public string fileName { get; private set; }
        public string? createdById { get; private set; }
        public DateTime? createdDate { get; private set; }

        public Attachment() { }

        public Attachment(string doc_company, DateTime integrated_or_updated_at, long ticketId, int actionId, Dtos.Reads.Tickets.Attachment dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.actionId = actionId;

            path = ConverterValidatorsExtensions.StringLengthValidation(dto.path, 255); // hash do arquivo, chave da tabela
            fileName = ConverterValidatorsExtensions.StringLengthValidation(dto.fileName, 255);
            createdById = dto.createdBy?.id;
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
        }
    }
}
