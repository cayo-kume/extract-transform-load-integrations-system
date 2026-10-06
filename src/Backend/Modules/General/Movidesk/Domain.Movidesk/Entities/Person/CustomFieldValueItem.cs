using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Person
{
    public class CustomFieldValueItem
    {
        public string personId { get; private set; }
        public int customFieldId { get; private set; }
        public int clientId { get; private set; }
        public string team { get; private set; }
        public string customFieldItem { get; private set; }

        public CustomFieldValueItem() { }

        public CustomFieldValueItem(string personId, int customFieldId, Dtos.Person.CustomFieldValueItem customFieldValueItemDto)
        {
            this.personId = personId;
            this.customFieldId = customFieldId;

            clientId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueItemDto.clientId);

            team = customFieldValueItemDto.team;
            customFieldItem = customFieldValueItemDto.customFieldItem;
        }
    }
}
