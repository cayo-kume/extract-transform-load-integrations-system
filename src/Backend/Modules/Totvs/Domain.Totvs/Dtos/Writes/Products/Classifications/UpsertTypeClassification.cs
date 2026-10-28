namespace Domain.Totvs.Dtos.Writes.Products.Classifications
{
    /// <summary>
    /// corpo do POST product/v2/classifications (UpsertTypeClassificationCommand): inclui ou altera um tipo de classificação e as
    /// classificações dele. Renomear muda relatórios e a categorização de todos os produtos que usam o tipo. O tipo 6000 (marca) é recusado (PR10)
    /// </summary>
    public class UpsertTypeClassification
    {
        public int? typeCode { get; init; }
        public string? description { get; init; }
        public string? auxiliaryDescription { get; init; }
        public List<UpsertClassification>? classifications { get; init; }
    }
}
