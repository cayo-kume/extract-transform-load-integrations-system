using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class Tracking
    {
        public string? doc_company { get; private set; }

        public Int64 id { get; private set; }
        public Int64 reverse_id { get; private set; }

        public int courier_contract_id { get; private set; }

        public decimal shipping_amount { get; private set; }
        public decimal package_amount { get; private set; }

        public bool is_change_collect_to_post { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime? updated_at { get; private set; }
        public DateTime? deleted_at { get; private set; }
        public DateTime? status_updated_at { get; private set; }
        public DateTime? delivery_deadline { get; private set; }
        public DateTime? requested_collect_date { get; private set; }
        public DateTime? expire_date { get; private set; }
        public DateTime? collect_date { get; private set; }

        public string? authorization_code { get; private set; }
        public string? tracking_code { get; private set; }
        public string? courier_name { get; private set; }
        public string? tracking_url { get; private set; }
        public string? status { get; private set; }
        public string? message { get; private set; }
        public string? type { get; private set; }
        public string? qr_code { get; private set; }
        public string? service_type { get; private set; }
        public string? cte { get; private set; }

        [SkipProperty]
        public List<TrackingHistory> tracking_history { get; private set; } = [];

        public Tracking() { }

        public Tracking(string doc_company, Dtos.Reads.Reverses.GetReverse.Tracking trackingDto, List<Dtos.Reads.Reverses.GetReverse.TrackingHistory> trackingHistoriesDto)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(trackingDto.id);
            reverse_id = ConverterValidatorsExtensions.ConvertToInt64Validation(trackingDto.reverse_id);

            courier_contract_id = ConverterValidatorsExtensions.ConvertToInt32Validation(trackingDto.courier_contract_id);

            shipping_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(trackingDto.shipping_amount);
            package_amount = ConverterValidatorsExtensions.ConvertToDecimalValidation(trackingDto.package_amount);

            is_change_collect_to_post = ConverterValidatorsExtensions.ConvertToBooleanValidation(trackingDto.is_change_collect_to_post);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            updated_at = NullableConverters.ToNullableDateTime(trackingDto.updated_at);
            deleted_at = NullableConverters.ToNullableDateTime(trackingDto.deleted_at);
            status_updated_at = NullableConverters.ToNullableDateTime(trackingDto.status_updated_at);
            delivery_deadline = NullableConverters.ToNullableDateTime(trackingDto.delivery_deadline);
            requested_collect_date = NullableConverters.ToNullableDateTime(trackingDto.requested_collect_date);
            expire_date = NullableConverters.ToNullableDateTime(trackingDto.expire_date);
            collect_date = NullableConverters.ToNullableDateTime(trackingDto.collect_date);

            authorization_code = trackingDto.authorization_code?.Length > 50 ? trackingDto.authorization_code?.Substring(0, 50) : trackingDto.authorization_code;
            tracking_code = trackingDto.tracking_code?.Length > 50 ? trackingDto.tracking_code?.Substring(0, 50) : trackingDto.tracking_code;
            courier_name = trackingDto.courier_name?.Length > 250 ? trackingDto.courier_name?.Substring(0, 250) : trackingDto.courier_name;
            tracking_url = trackingDto.tracking_url?.Length > 300 ? trackingDto.tracking_url?.Substring(0, 300) : trackingDto.tracking_url;
            status = trackingDto.status?.Length > 50 ? trackingDto.status?.Substring(0, 50) : trackingDto.status;
            message = trackingDto.message?.Length > 800 ? trackingDto.message?.Substring(0, 800) : trackingDto.message;
            type = trackingDto.type?.Length > 50 ? trackingDto.type?.Substring(0, 50) : trackingDto.type;
            qr_code = trackingDto.qr_code?.Length > 800 ? trackingDto.qr_code?.Substring(0, 800) : trackingDto.qr_code;
            service_type = trackingDto.service_type?.Length > 50 ? trackingDto.service_type?.Substring(0, 50) : trackingDto.service_type;
            cte = trackingDto.cte?.Length > 50 ? trackingDto.cte?.Substring(0, 50) : trackingDto.cte;

            if (trackingHistoriesDto != null && trackingHistoriesDto?.Count() > 0)
            {
                trackingHistoriesDto.ForEach(x => {
                    tracking_history.Add(new TrackingHistory(doc_company, x, reverse_id));
                });
            }
        }
    }
}
