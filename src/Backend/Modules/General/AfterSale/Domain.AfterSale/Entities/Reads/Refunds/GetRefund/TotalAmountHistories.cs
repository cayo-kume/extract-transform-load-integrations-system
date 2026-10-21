using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class TotalAmountHistories
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public Int64 id { get; private set; }
        public Int64 user_id { get; private set; }

        public int type_id { get; private set; }
        
        public DateTime date { get; private set; }
        
        public string? user_first_name { get; private set; }
        public string? user_last_name { get; private set; }
        public string? total_amount { get; private set; }
        public string? type_name { get; private set; }
        public string? type_description { get; private set; }

        public TotalAmountHistories() { }

        public TotalAmountHistories(Dtos.Reads.Refunds.GetRefund.TotalAmountHistories totalAmountHistoriesDto, Int64 refund_id)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.refund_id = refund_id;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(totalAmountHistoriesDto.id);
            user_id = ConverterValidatorsExtensions.ConvertToInt64Validation(totalAmountHistoriesDto.user?.id);

            type_id = ConverterValidatorsExtensions.ConvertToInt32Validation(totalAmountHistoriesDto.type?.id);

            date = ConverterValidatorsExtensions.ConvertToDateTimeValidation(totalAmountHistoriesDto.date);

            user_first_name = totalAmountHistoriesDto.user?.first_name?.Length > 50 ? totalAmountHistoriesDto.user?.first_name?.Substring(0, 50) : totalAmountHistoriesDto.user?.first_name;
            user_last_name = totalAmountHistoriesDto.user?.last_name?.Length > 50 ? totalAmountHistoriesDto.user?.last_name?.Substring(0, 50) : totalAmountHistoriesDto.user?.last_name;
            total_amount = totalAmountHistoriesDto.total_amount?.Length > 50 ? totalAmountHistoriesDto.total_amount?.Substring(0, 50) : totalAmountHistoriesDto.total_amount;
            type_name = totalAmountHistoriesDto.type?.name?.Length > 50 ? totalAmountHistoriesDto.type?.name?.Substring(0, 50) : totalAmountHistoriesDto.type?.name;
            type_description = totalAmountHistoriesDto.type?.description?.Length > 50 ? totalAmountHistoriesDto.type?.description?.Substring(0, 50) : totalAmountHistoriesDto.type?.description;
        }
    }
}
