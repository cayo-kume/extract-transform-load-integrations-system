namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// região de atuação do GET seller/v2/operational-area (OperationalAreaModel)
    /// </summary>
    public class OperationalArea
    {
        public string? operationalAreaCode { get; init; }
        public string? operationalAreaName { get; init; }
        public string? operationalAreaType { get; init; }
        public string? operationalAreaGroupCode { get; init; }
        public string? operationalAreaGroupDescription { get; init; }
        public string? numberOfDays { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
