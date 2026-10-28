namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// preço ou custo na inclusão (ProductValueInsertModel): recusado pelo validador até a PR7 (PR11); valueType C ou P
    /// </summary>
    public class ProductValueInsert
    {
        public int? branchCode { get; init; }
        public string? valueType { get; init; }
        public int? valueCode { get; init; }
        public decimal? value { get; init; }
    }
}
