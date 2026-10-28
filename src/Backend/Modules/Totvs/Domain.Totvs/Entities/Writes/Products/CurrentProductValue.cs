namespace Domain.Totvs.Entities.Writes.Products
{
    /// <summary>
    /// preço (P, de totvs.TotvsPrices) ou custo (C, de totvs.TotvsCosts) gravado de um produto numa empresa: base da trava de variação (PR7)
    /// </summary>
    public class CurrentProductValue
    {
        public int branchCode { get; set; }
        public int productCode { get; set; }
        public string valueType { get; set; } = String.Empty;
        public int valueCode { get; set; }
        public decimal? value { get; set; }
    }
}
