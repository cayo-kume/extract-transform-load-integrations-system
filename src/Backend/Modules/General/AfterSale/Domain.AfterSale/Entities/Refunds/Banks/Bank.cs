using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Refunds.Banks
{
    public class Bank
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public int id { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Bank() { }

        public Bank(Dtos.Refunds.Banks.Bank bankDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            id = ConverterValidatorsExtensions.ConvertToInt32Validation(bankDto.id);

            name = bankDto.name?.Length > 50 ? bankDto.name?.Substring(0, 50) : bankDto.name;
            description = bankDto.description?.Length > 250 ? bankDto.description?.Substring(0, 250) : bankDto.description;
        }
    }
}
