namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// SkuParentRelation do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SkuParentRelation
    {
        public string? ParentID { get; init; }
        public string? ParentSKU { get; init; }
        public string? ParentIntegrationID { get; init; }
    }
}
