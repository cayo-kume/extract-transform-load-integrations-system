using Domain.Shared.Extensions;

namespace Domain.AfterSale.Entities.Reads.Me
{
    public class SubReason
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int reason_id { get; private set; }
        public int ecommerce_id { get; private set; }

        public string? description { get; private set; }
        public string? ord { get; private set; }

        public DateTime? created_at { get; private set; }
        public DateTime? updated_at { get; private set; }
        public DateTime? deleted_at { get; private set; }

        public SubReason() { }

        public SubReason(Dtos.Reads.Me.SubReason subReasonDto, int ecommerce_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.ecommerce_id = ecommerce_id;
            id = ConverterValidatorsExtensions.ConvertToInt32Validation(subReasonDto.id);
            reason_id = ConverterValidatorsExtensions.ConvertToInt32Validation(subReasonDto.reason_id);

            description = subReasonDto.description?.Length > 50 ? subReasonDto.description.Substring(0, 50) : subReasonDto.description;
            ord = subReasonDto.ord?.Length > 50 ? subReasonDto.ord.Substring(0, 60) : subReasonDto.ord;

            created_at = NullableConverters.ToNullableDateTime(subReasonDto.created_at);
            updated_at = NullableConverters.ToNullableDateTime(subReasonDto.updated_at);
            deleted_at = NullableConverters.ToNullableDateTime(subReasonDto.deleted_at);
        }
    }
}