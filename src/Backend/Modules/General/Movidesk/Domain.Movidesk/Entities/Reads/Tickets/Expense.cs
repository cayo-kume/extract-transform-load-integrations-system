using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Expense
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int actionId { get; private set; }
        public long id { get; private set; }
        public string type { get; private set; }
        public string serviceReport { get; private set; }
        public string? createdById { get; private set; }
        public int? createdByTeamId { get; private set; }
        public string createdByTeamName { get; private set; }
        public DateTime? date { get; private set; }
        public int? quantity { get; private set; }
        public decimal? value { get; private set; }

        public Expense() { }

        public Expense(string doc_company, DateTime integrated_or_updated_at, long ticketId, int actionId, Dtos.Reads.Tickets.Expense dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.actionId = actionId;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 128);
            serviceReport = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceReport, 128);
            createdById = dto.createdBy?.id;
            createdByTeamId = NullableConverters.ToNullableInt32(dto.createdByTeam?.id);
            createdByTeamName = ConverterValidatorsExtensions.StringLengthValidation(dto.createdByTeam?.name, 128);
            date = NullableConverters.ToNullableDateTime(dto.date);
            quantity = NullableConverters.ToNullableInt32(dto.quantity);
            value = NullableConverters.ToNullableDecimal(dto.value);
        }
    }
}
