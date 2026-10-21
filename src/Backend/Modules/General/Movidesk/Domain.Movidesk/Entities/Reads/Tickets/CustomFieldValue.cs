using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class CustomFieldValue
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public int customFieldId { get; private set; }
        public int customFieldRuleId { get; private set; }
        public int line { get; private set; }
        public string? value { get; private set; }

        [SkipProperty]
        public List<CustomFieldValueItem> items { get; private set; } = [];

        public CustomFieldValue() { }

        public CustomFieldValue(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.CustomFieldValue dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;

            customFieldId = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.customFieldId);
            customFieldRuleId = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.customFieldRuleId);
            line = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.line);
            value = dto.value; // nvarchar(max): texto livre (datas, números com vírgula ou ponto)

            // itens não têm id na API: posição na lista
            var dtoItems = dto.items ?? [];
            for (int i = 0; i < dtoItems.Count; i++)
                items.Add(new CustomFieldValueItem(doc_company, integrated_or_updated_at, ticketId, customFieldId, customFieldRuleId, line, i + 1, dtoItems[i]));
        }
    }
}
