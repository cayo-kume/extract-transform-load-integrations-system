namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// referência (ReferenceInDto)
    /// </summary>
    public class ReferenceIn
    {
        public int? sequence { get; init; }
        public string? type { get; init; }
        public string? description { get; init; }
        public string? phoneNumber { get; init; }
        public string? responsiblePersonName { get; init; }
        public bool? isInactive { get; init; }
    }
}
