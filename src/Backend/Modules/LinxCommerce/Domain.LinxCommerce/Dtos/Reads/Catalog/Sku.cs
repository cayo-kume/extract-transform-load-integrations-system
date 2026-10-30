using Newtonsoft.Json.Linq;

namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// Sku do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class Sku
    {
        public string? ProductID { get; init; }
        public string? Name { get; init; }
        public string? SKU { get; init; }
        public string? IntegrationID { get; init; }
        public string? UPC { get; init; }
        public string? DisplayCondition { get; init; }
        public string? DefinitionID { get; init; }
        public string? ConditionID { get; init; }
        public string? UnitOfMeasureID { get; init; }
        public string? ManageStock { get; init; }
        public string? MinimumQtyAllowed { get; init; }
        public string? MaximumQtyAllowed { get; init; }
        public string? Preorderable { get; init; }
        public string? PreorderDate { get; init; }
        public string? PreorderLimit { get; init; }
        public string? Backorderable { get; init; }
        public string? BackorderLimit { get; init; }
        public string? PurchasingPolicyID { get; init; }
        public string? WrappingQty { get; init; }
        public string? Weight { get; init; }
        public string? Height { get; init; }
        public string? Width { get; init; }
        public string? Depth { get; init; }
        public string? Cost { get; init; }
        public string? Tax { get; init; }
        public string? IsVisible { get; init; }
        public string? VisibleFrom { get; init; }
        public string? VisibleTo { get; init; }
        public string? IsDeleted { get; init; }
        public string? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public string? ModifiedDate { get; init; }
        public string? ModifiedBy { get; init; }

        public JToken? SuppliersIDs { get; init; }
        public JToken? ParentsIDs { get; init; }
        public List<SkuMetadataValue>? MetadataValues { get; init; }
        public List<SkuParentRelation>? ParentRelations { get; init; }
    }
}
