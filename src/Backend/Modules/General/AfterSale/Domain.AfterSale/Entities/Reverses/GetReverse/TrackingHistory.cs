using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reverses.GetReverse
{
    public class TrackingHistory
    {
        public Int64 id { get; private set; }
        public Int64 reverse_id { get; private set; }
        public Int64 tracking_id { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime status_updated_at { get; private set; }
        public DateTime created_at { get; private set; }
        public DateTime updated_at { get; private set; }
        public DateTime deleted_at { get; private set; }

        public string? status { get; private set; }
        public string? message { get; private set; }

        public TrackingHistory() { }

        public TrackingHistory(Dtos.Reverses.GetReverse.TrackingHistory trackingHistoryDto, Int64 reverse_id)
        {
            this.reverse_id = reverse_id;
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(trackingHistoryDto.id);
            tracking_id = ConverterValidatorsExtensions.ConvertToInt64Validation(trackingHistoryDto.tracking_id);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            status_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(trackingHistoryDto.status_updated_at);
            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(trackingHistoryDto.created_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(trackingHistoryDto.updated_at);
            deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(trackingHistoryDto.deleted_at);

            status = trackingHistoryDto.status?.Length > 50 ? trackingHistoryDto.status?.Substring(0, 50) : trackingHistoryDto.status;
            message = trackingHistoryDto.message?.Length > 800 ? trackingHistoryDto.message?.Substring(0, 800) : trackingHistoryDto.message;
        }
    }
}
