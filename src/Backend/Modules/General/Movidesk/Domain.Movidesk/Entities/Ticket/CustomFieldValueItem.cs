using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Ticket
{
    public class CustomFieldValueItem
    {
        public string ticketId { get; private set; }
        public string team { get; private set; }
        public string customFieldItem { get; private set; }

        public int id { get; private set; }
        public int customFieldId { get; private set; }
        public int personId { get; private set; }
        public int clientId { get; private set; }

        public CustomFieldValueItem() { }

        public CustomFieldValueItem(int id, string ticketId, int customFieldId, Dtos.Tickets.CustomFieldValueItem customFieldValueItemDto)
        {
            this.ticketId = ticketId;
            this.customFieldId = customFieldId;
            this.id = id;

            team = customFieldValueItemDto.team;
            customFieldItem = customFieldValueItemDto.customFieldItem;
            personId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueItemDto.personId);
            clientId = ConverterValidatorsExtensions.ConvertToInt32Validation(customFieldValueItemDto.clientId);
        }
    }
}
