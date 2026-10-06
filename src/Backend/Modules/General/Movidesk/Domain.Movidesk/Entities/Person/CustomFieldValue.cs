using Domain.Core.Extensions;
//using Domain.Movidesk.Models.Tickets;

namespace Domain.Movidesk.Entities.Person
{
    public class CustomFieldValue
    {
        public string personId { get; private set; }
        public int customFieldId { get; private set; }
        public int customFieldRuleId { get; private set; }
        public int line { get; private set; }
        public string value { get; private set; }

        [SkipProperty]
        public List<CustomFieldValueItem> items { get; private set; } = [];

        public CustomFieldValue() { }

        public CustomFieldValue(string personId, Dtos.Person.CustomFieldValue customFieldValueDto)
        {
            this.personId = personId;

            customFieldId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.customFieldId);
            customFieldRuleId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.customFieldRuleId);
            line = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueDto.line);
            value = customFieldValueDto.value;

            customFieldValueDto.items?.ForEach(item =>
                items.Add(new CustomFieldValueItem(personId, customFieldId, item)
            ));
        }
    }
}
