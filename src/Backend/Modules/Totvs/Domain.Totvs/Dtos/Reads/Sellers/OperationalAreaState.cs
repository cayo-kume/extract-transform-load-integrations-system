namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// estado de uma região de atuação do GET seller/v2/operational-area-state (OperationalAreaStateModel)
    /// </summary>
    public class OperationalAreaState
    {
        public string? stateCode { get; init; }
        public string? stateName { get; init; }
        public string? operationalAreaCode { get; init; }
        public string? operationalAreaName { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
