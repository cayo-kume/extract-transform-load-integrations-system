namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// cidade de uma região de atuação do GET seller/v2/operational-area-city (OperationalAreaCityModel)
    /// </summary>
    public class OperationalAreaCity
    {
        public string? cityCode { get; init; }
        public string? cityName { get; init; }
        public string? stateCode { get; init; }
        public string? stateAbbreviation { get; init; }
        public string? stateName { get; init; }
        public string? operationalAreaCode { get; init; }
        public string? operationalAreaName { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
