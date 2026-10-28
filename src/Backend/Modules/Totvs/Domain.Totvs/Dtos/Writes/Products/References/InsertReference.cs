namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// corpo do POST product/v2/references (InsertReferenceCommand): inclui uma referência com os SKUs; a resposta traz os
    /// productCode gerados. Cria produto vendável, sem rota de exclusão. Os preços (values) são recusados até a PR7 (PR11)
    /// </summary>
    public class InsertReference
    {
        public int? referenceSettingsCode { get; init; }
        // vazio: a máscara monta o nome
        public string? referenceName { get; init; }
        public int? gridCode { get; init; }
        public string? observation { get; init; }
        // um por nível da máscara
        public List<ReferenceGroup>? groups { get; init; }
        // grafia da API
        public List<ReferenceBranch>? branchs { get; init; }
        public List<InsertProduct>? skus { get; init; }
        public List<ProductValueInsert>? values { get; init; }
        public List<ProductImageInsert>? images { get; init; }
        public ReferenceWeb? referenceWeb { get; init; }
    }
}
