namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// valor de um campo adicional da pessoa
    /// </summary>
    public class CustomFieldValue
    {
        public int? customFieldId { get; init; }

        public int? customFieldRuleId { get; init; }

        // 1 quando a regra não permite adicionar linhas
        public int? line { get; init; }

        // campos de texto, número, data etc.
        public string? value { get; init; }

        // campos do tipo lista
        public List<CustomFieldValueItem>? items { get; init; }
    }
}
