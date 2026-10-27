namespace Domain.Totvs.Dtos.Reads.People.Representatives
{
    /// <summary>
    /// região de venda do representante (RepresentativeSaleRegionModel)
    /// </summary>
    public class RepresentativeSalesRegion
    {
        public string? salesRegionCode { get; init; }
        public string? salesRegionName { get; init; }
        public string? salesOrderClassificationTypeCode { get; init; }
        public string? salesOrderClassificationTypeName { get; init; }
        public string? salesOrderClassificationCode { get; init; }
        public string? salesOrderClassificationName { get; init; }
    }
}
