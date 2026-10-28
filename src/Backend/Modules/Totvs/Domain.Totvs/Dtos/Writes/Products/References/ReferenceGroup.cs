namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// nível do grupo da referência (GroupModel)
    /// </summary>
    public class ReferenceGroup
    {
        public int? levelNumber { get; init; }
        public string? groupCode { get; init; }
        public string? groupName { get; init; }
    }
}
