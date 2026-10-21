using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Action
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int id { get; private set; }
        public int? type { get; private set; }
        public int? origin { get; private set; }
        public string? description { get; private set; }
        public string? htmlDescription { get; private set; }
        public string status { get; private set; }
        public string justification { get; private set; }
        public DateTime? createdDate { get; private set; }
        public string? createdById { get; private set; }
        public bool isDeleted { get; private set; }

        [SkipProperty]
        public List<ActionTag> tags { get; private set; } = [];

        [SkipProperty]
        public List<TimeAppointment> timeAppointments { get; private set; } = [];

        [SkipProperty]
        public List<Expense> expenses { get; private set; } = [];

        [SkipProperty]
        public List<Attachment> attachments { get; private set; } = [];

        public Action() { }

        public Action(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.Action dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.id);
            type = NullableConverters.ToNullableInt32(dto.type);
            origin = NullableConverters.ToNullableInt32(dto.origin);
            description = dto.description;          // nvarchar(max)
            htmlDescription = dto.htmlDescription;  // só vem na busca por id; na listagem fica nulo
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 128);
            justification = ConverterValidatorsExtensions.StringLengthValidation(dto.justification, 128);
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
            createdById = dto.createdBy?.id;
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);

            foreach (var tag in (dto.tags ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
                tags.Add(new ActionTag(doc_company, integrated_or_updated_at, ticketId, id, tag));

            foreach (var appointment in (dto.timeAppointments ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
                timeAppointments.Add(new TimeAppointment(doc_company, integrated_or_updated_at, ticketId, id, appointment));

            foreach (var expense in (dto.expenses ?? []).Where(e => !String.IsNullOrEmpty(e.id)))
                expenses.Add(new Expense(doc_company, integrated_or_updated_at, ticketId, id, expense));

            foreach (var attachment in (dto.attachments ?? []).Where(a => !String.IsNullOrEmpty(a.path)))
                attachments.Add(new Attachment(doc_company, integrated_or_updated_at, ticketId, id, attachment));
        }

        /// <summary>
        /// a listagem da API não traz htmlDescription: mantém o que já estava gravado em vez de sobrescrever com null
        /// </summary>
        public void KeepHtmlDescription(string? storedHtmlDescription)
        {
            if (String.IsNullOrEmpty(htmlDescription))
                htmlDescription = storedHtmlDescription;
        }
    }
}
