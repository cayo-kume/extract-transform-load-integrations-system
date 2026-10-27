namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// unidade de medida, a espécie (GET product/v2/measurement-unit, MeasurementUnitModel)
    /// </summary>
    public class MeasurementUnit
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<MeasurementUnitVariation>? additionalVariations { get; init; }
    }
}
