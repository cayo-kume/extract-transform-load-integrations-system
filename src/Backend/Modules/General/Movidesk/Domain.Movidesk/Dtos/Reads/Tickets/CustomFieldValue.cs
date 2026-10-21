namespace Domain.Movidesk.Dtos.Reads.Tickets
{
    public class CustomFieldValue
    {
        public string customFieldId { get; init; }
        public string customFieldRuleId { get; init; }
        public string line { get; init; }
        public string value { get; init; }

        public List<CustomFieldValueItem> items { get; init; }
    }
}
