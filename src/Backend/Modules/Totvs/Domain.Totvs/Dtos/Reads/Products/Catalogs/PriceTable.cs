namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// cabeçalho de tabela de preço (GET product/v2/price-tables-headers, PriceTablesHeadersModel)
    /// </summary>
    public class PriceTable
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? codeName { get; init; }
        public string? type { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? startDate { get; init; }
        public string? endDate { get; init; }
        public string? variationValue { get; init; }
        public string? variationPercentage { get; init; }
        public List<PriceTableSalesOrderClassification>? salesOrderClassification { get; init; }
        public List<PriceTablePersonClassification>? personClassification { get; init; }
        public List<PriceTablePaymentCondition>? paymentConditions { get; init; }
        public List<PriceTableAveragePeriod>? averagePeriod { get; init; }
        public List<PriceTableAveragePeriodQuantity>? averagePeriodQuantity { get; init; }
    }
}
