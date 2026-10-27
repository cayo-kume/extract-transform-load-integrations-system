namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// prazo médio por quantidade da tabela de preço, na posição em que a API o devolve (AveragePeriodQuantityModel, sem chave)
    /// </summary>
    public class PriceTableAveragePeriodQuantity
    {
        public string? averagePeriod { get; init; }
        public string? variationQuantity { get; init; }
        public string? variationPercentage { get; init; }
    }
}
