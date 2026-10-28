namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// campo adicional da duplicata (DuplicateFieldModel): o dado vai no campo do tipo dele (texto, número ou data)
    /// </summary>
    public class DuplicateField
    {
        public int? typeFieldCode { get; init; }
        public string? descriptionField { get; init; }
        public decimal? numericField { get; init; }
        public DateTime? dateField { get; init; }
    }
}
