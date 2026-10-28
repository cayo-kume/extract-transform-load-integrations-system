namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// dados web da referência (InsertReferenteWebModel): medidas com 9 inteiros e 3 decimais
    /// </summary>
    public class ReferenceWeb
    {
        public ProductWebStatusType? status { get; init; }
        public string? description { get; init; }
        public decimal? weight { get; init; }
        public decimal? height { get; init; }
        public decimal? width { get; init; }
        public decimal? length { get; init; }
        public decimal? packWeight { get; init; }
        public decimal? packHeight { get; init; }
        public decimal? packWidth { get; init; }
        public decimal? packLength { get; init; }
        public List<ReferenceWebDetail>? descriptiveList { get; init; }
    }
}
