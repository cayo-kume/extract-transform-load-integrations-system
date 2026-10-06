using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Sku
{
    public class Sku
    {
        public DateTime lastupdateon { get; set; }

        public string? UPC { get; set; }

        public bool? DisplayCondition { get; set; }

        public Int32? DefinitionID { get; set; }

        public string? SuppliersIDs { get; set; }

        public string? ParentsIDs { get; set; }

        public Int32? ConditionID { get; set; }

        public Int32? UnitOfMeasureID { get; set; }

        public bool? ManageStock { get; set; }

        public decimal? MinimumQtyAllowed { get; set; }

        public decimal? MaximumQtyAllowed { get; set; }

        public bool? Preorderable { get; set; }

        public DateTimeOffset? PreorderDate { get; set; }

        public decimal? PreorderLimit { get; set; }

        public bool? Backorderable { get; set; }

        public decimal? BackorderLimit { get; set; }

        public Int32? PurchasingPolicyID { get; set; }

        public string? WrappingQty { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        public decimal? Width { get; set; }

        public decimal? Depth { get; set; }

        public decimal? Cost { get; set; }

        public decimal? Tax { get; set; }

        public bool? IsVisible { get; set; }

        public DateTimeOffset? VisibleFrom { get; set; }

        public DateTimeOffset? VisibleTo { get; set; }

        public bool? IsDeleted { get; set; }

        public Int32? ProductID { get; set; }

        public string? Name { get; set; }

        public string? SKU { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? IntegrationID { get; set; }

        [SkipProperty]
        public List<string?> SuppliersID { get; set; }

        [SkipProperty]
        public List<string?> ParentsID { get; set; }

        [SkipProperty]
        public List<MetadataValue> MetadataValues { get; set; } = new List<MetadataValue>();

        [SkipProperty]
        public List<ParentRelation> ParentRelations { get; set; } = new List<ParentRelation>();

        [SkipProperty]
        public Dictionary<int?, string> Responses { get; set; } = new Dictionary<int?, string>();

        public Sku() { }

        public Sku(Sku sku, string? getSkuResponse)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.UPC = sku.UPC;
            this.DisplayCondition = sku.DisplayCondition;
            this.DefinitionID = sku.DefinitionID;
            this.SuppliersIDs = sku.SuppliersIDs;
            this.ParentsIDs = sku.ParentsIDs;
            this.ConditionID = sku.ConditionID;
            this.UnitOfMeasureID = sku.UnitOfMeasureID;
            this.ManageStock = sku.ManageStock;
            this.MinimumQtyAllowed = sku.MinimumQtyAllowed;
            this.MaximumQtyAllowed = sku.MaximumQtyAllowed;
            this.Preorderable = sku.Preorderable;
            this.PreorderDate = sku.PreorderDate;
            this.PreorderLimit = sku.PreorderLimit;
            this.Backorderable = sku.Backorderable;
            this.BackorderLimit = sku.BackorderLimit;
            this.PurchasingPolicyID = sku.PurchasingPolicyID;
            this.WrappingQty = sku.WrappingQty;
            this.Weight = sku.Weight;
            this.Height = sku.Height;
            this.Width = sku.Width;
            this.Depth = sku.Depth;
            this.Cost = sku.Cost;
            this.Tax = sku.Tax;
            this.IsVisible = sku.IsVisible;
            this.VisibleFrom = sku.VisibleFrom;
            this.VisibleTo = sku.VisibleTo;
            this.IsDeleted = sku.IsDeleted;
            this.ProductID = sku.ProductID;
            this.Name = sku.Name;
            this.SKU = sku.SKU;
            this.CreatedDate = sku.CreatedDate;
            this.CreatedBy = sku.CreatedBy;
            this.ModifiedDate = sku.ModifiedDate;
            this.ModifiedBy = sku.ModifiedBy;
            this.IntegrationID = sku.IntegrationID;
            this.Responses.Add(sku.ProductID, getSkuResponse);

            foreach (var metadataValue in sku.MetadataValues)
            {
                this.MetadataValues.Add(new MetadataValue(metadataValue, sku.ProductID));
            }

            foreach (var parentRelation in sku.ParentRelations)
            {
                this.ParentRelations.Add(new ParentRelation(parentRelation, sku.ProductID));
            }
        }
    }
}
