using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.Actions
{
    public class Action
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public int id { get; private set; }
        public string? doc_company { get; private set; }
        public string? name { get; private set; }
        public string? description { get; private set; }

        public Action() { }

        public Action(Dtos.Reads.Refunds.Actions.Action actionDto, string doc_company)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(actionDto.id);

            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;
            name = actionDto.name?.Length > 50 ? actionDto.name?.Substring(0, 50) : actionDto.name;
            description = actionDto.description?.Length > 250 ? actionDto.description?.Substring(0, 250) : actionDto.description;
        }
    }
}
