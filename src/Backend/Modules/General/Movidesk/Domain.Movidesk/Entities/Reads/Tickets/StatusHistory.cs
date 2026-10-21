using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class StatusHistory
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int id { get; private set; }
        public string status { get; private set; }
        public string justification { get; private set; }
        public decimal? permanencyTimeFullTime { get; private set; }
        public decimal? permanencyTimeWorkingTime { get; private set; }
        public string? changedById { get; private set; }
        public DateTime? changedDate { get; private set; }

        public StatusHistory() { }

        public StatusHistory(string doc_company, DateTime integrated_or_updated_at, long ticketId, int id, Dtos.Reads.Tickets.StatusHistory dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.id = id;

            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 128);
            justification = ConverterValidatorsExtensions.StringLengthValidation(dto.justification, 128);
            permanencyTimeFullTime = NullableConverters.ToNullableDecimal(dto.permanencyTimeFullTime);
            permanencyTimeWorkingTime = NullableConverters.ToNullableDecimal(dto.permanencyTimeWorkingTime);
            changedById = dto.changedBy?.id;
            changedDate = NullableConverters.ToNullableDateTime(dto.changedDate);
        }
    }
}
