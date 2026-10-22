using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class StatusHistories
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public string? doc_company { get; private set; }

        public Int64 refund_id { get; private set; }
        public Int64 user_id { get; private set; }

        public int status_id { get; private set; }

        public DateTime date { get; private set; }

        public string? status_service_code { get; private set; }
        public string? status_name { get; private set; }
        public string? status_description { get; private set; }
        public string? user_name { get; private set; }
        public string? user_first_name { get; private set; }
        public string? user_last_name { get; private set; }
        public string? user_email { get; private set; }
        public string? user_mobile_phone { get; private set; }
        public string? user_phone { get; private set; }
        public string? user_document { get; private set; }
        public string? user_wallet { get; private set; }
        public string? comments { get; private set; }

        public StatusHistories() { }

        public StatusHistories(string doc_company, Dtos.Reads.Refunds.GetRefund.StatusHistories statusHistoriesDto, Int64 refund_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.refund_id = refund_id;
            user_id = ConverterValidatorsExtensions.ConvertToInt64Validation(statusHistoriesDto.user?.id);
            status_id = ConverterValidatorsExtensions.ConvertToInt32Validation(statusHistoriesDto.status?.id);
            date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(statusHistoriesDto.date);

            status_service_code = statusHistoriesDto.status?.service_code?.Length > 50 ? statusHistoriesDto.status?.service_code?.Substring(0, 50) : statusHistoriesDto.status?.service_code;
            status_name = statusHistoriesDto.status?.name?.Length > 50 ? statusHistoriesDto.status?.name?.Substring(0, 50) : statusHistoriesDto.status?.name;
            status_description = statusHistoriesDto.status?.description?.Length > 50 ? statusHistoriesDto.status?.description?.Substring(0, 50) : statusHistoriesDto.status?.description;
            user_name = statusHistoriesDto.user?.name?.Length > 50 ? statusHistoriesDto.user?.name?.Substring(0, 50) : statusHistoriesDto.user?.name;
            user_first_name = statusHistoriesDto.user?.first_name?.Length > 50 ? statusHistoriesDto.user?.first_name?.Substring(0, 50) : statusHistoriesDto.user?.first_name;
            user_last_name = statusHistoriesDto.user?.last_name?.Length > 100 ? statusHistoriesDto.user?.last_name?.Substring(0, 100) : statusHistoriesDto.user?.last_name;
            user_email = statusHistoriesDto.user?.email?.Length > 50 ? statusHistoriesDto.user?.email?.Substring(0, 50) : statusHistoriesDto.user?.email;
            user_mobile_phone = statusHistoriesDto.user?.mobile_phone?.Length > 20 ? statusHistoriesDto.user?.mobile_phone?.Substring(0, 20) : statusHistoriesDto.user?.mobile_phone;
            user_phone = statusHistoriesDto.user?.phone?.Length > 20 ? statusHistoriesDto.user?.phone?.Substring(0, 20) : statusHistoriesDto.user?.phone;
            user_document = statusHistoriesDto.user?.document?.Length > 14 ? statusHistoriesDto.user?.document?.Substring(0, 14) : statusHistoriesDto.user?.document;
            user_wallet = statusHistoriesDto.user?.wallet?.Length > 50 ? statusHistoriesDto.user?.wallet?.Substring(0, 50) : statusHistoriesDto.user?.wallet;
            comments = statusHistoriesDto.comments?.Length > 300 ? statusHistoriesDto.comments?.Substring(0, 300) : statusHistoriesDto.comments;
        }
    }
}
