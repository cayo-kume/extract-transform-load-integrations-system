using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Ticket
{
    public class CustomFieldValue
    {
        public string ticketId { get; private set; }
        public int customFieldId { get; private set; }
        public int customFieldRuleId { get; private set; }
        public int line { get; private set; }
        public string value { get; private set; }

        [SkipProperty]
        public List<CustomFieldValueItem> items { get; private set; }

        public CustomFieldValue() { }

        public CustomFieldValue(Dtos.Tickets.CustomFieldValue customFieldValueDto, string ticketId)
        {
            this.ticketId = ticketId;

            customFieldId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.customFieldId);
            customFieldRuleId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.customFieldRuleId);
            line = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.line);
            value = customFieldValueDto.value;

            for (int i = 0; i <= customFieldValueDto.items?.Count(); i++)
            {
                items.Add(new CustomFieldValueItem(i + 1, ticketId, customFieldId, customFieldValueDto.items[i]));
            }
        }
    }
}
