using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class TimeAppointment
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int actionId { get; private set; }
        public long id { get; private set; }
        public string activity { get; private set; }
        public DateTime? date { get; private set; }
        public TimeSpan? periodStart { get; private set; }
        public TimeSpan? periodEnd { get; private set; }
        public TimeSpan? workTime { get; private set; }
        public decimal? accountedTime { get; private set; }
        public string workTypeName { get; private set; }
        public string? createdById { get; private set; }
        public int? createdByTeamId { get; private set; }
        public string createdByTeamName { get; private set; }

        public TimeAppointment() { }

        public TimeAppointment(string doc_company, DateTime integrated_or_updated_at, long ticketId, int actionId, Dtos.Reads.Tickets.TimeAppointment dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.actionId = actionId;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
            activity = ConverterValidatorsExtensions.StringLengthValidation(dto.activity, 128);
            date = NullableConverters.ToNullableDateTime(dto.date);
            periodStart = NullableConverters.ToNullableTimeSpan(dto.periodStart);
            periodEnd = NullableConverters.ToNullableTimeSpan(dto.periodEnd);
            workTime = NullableConverters.ToNullableTimeSpan(dto.workTime);
            accountedTime = NullableConverters.ToNullableDecimal(dto.accountedTime);
            workTypeName = ConverterValidatorsExtensions.StringLengthValidation(dto.workTypeName, 128);
            createdById = dto.createdBy?.id;
            createdByTeamId = NullableConverters.ToNullableInt32(dto.createdByTeam?.id);
            createdByTeamName = ConverterValidatorsExtensions.StringLengthValidation(dto.createdByTeam?.name, 128);
        }
    }
}
