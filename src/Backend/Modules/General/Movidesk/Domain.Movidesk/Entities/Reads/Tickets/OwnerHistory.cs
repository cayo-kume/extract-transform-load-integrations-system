using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class OwnerHistory
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int id { get; private set; }
        public string ownerTeam { get; private set; }
        public string? ownerId { get; private set; }
        public decimal? permanencyTimeFullTime { get; private set; }
        public decimal? permanencyTimeWorkingTime { get; private set; }
        public string? changedById { get; private set; }
        public DateTime? changedDate { get; private set; }

        public OwnerHistory() { }

        public OwnerHistory(string doc_company, DateTime integrated_or_updated_at, long ticketId, int id, Dtos.Reads.Tickets.OwnerHistory dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;
            this.id = id;

            ownerTeam = ConverterValidatorsExtensions.StringLengthValidation(dto.ownerTeam, 128);
            ownerId = dto.owner?.id;
            permanencyTimeFullTime = NullableConverters.ToNullableDecimal(dto.permanencyTimeFullTime);
            permanencyTimeWorkingTime = NullableConverters.ToNullableDecimal(dto.permanencyTimeWorkingTime);
            changedById = dto.changedBy?.id;
            changedDate = NullableConverters.ToNullableDateTime(dto.changedDate);
        }
    }
}
