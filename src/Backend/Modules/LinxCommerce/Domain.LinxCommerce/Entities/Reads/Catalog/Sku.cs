using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// SKU (POST Catalog/API.svc/web/GetSKU); o id do SKU é o ProductID, tabela linxcommerce.LinxCommerceSkus
    /// </summary>
    public class Sku
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string SKU { get; private set; } = String.Empty;
        public string IntegrationID { get; private set; } = String.Empty;
        public string UPC { get; private set; } = String.Empty;
        public bool? DisplayCondition { get; private set; }
        public int? DefinitionID { get; private set; }
        public int? ConditionID { get; private set; }
        public int? UnitOfMeasureID { get; private set; }
        public int? PurchasingPolicyID { get; private set; }
        public bool? ManageStock { get; private set; }
        public decimal? MinimumQtyAllowed { get; private set; }
        public decimal? MaximumQtyAllowed { get; private set; }
        public bool? Preorderable { get; private set; }
        public DateTimeOffset? PreorderDate { get; private set; }
        public decimal? PreorderLimit { get; private set; }
        public bool? Backorderable { get; private set; }
        public decimal? BackorderLimit { get; private set; }
        public string WrappingQty { get; private set; } = String.Empty;
        public decimal? Weight { get; private set; }
        public decimal? Height { get; private set; }
        public decimal? Width { get; private set; }
        public decimal? Depth { get; private set; }
        public decimal? Cost { get; private set; }
        public decimal? Tax { get; private set; }
        public bool? IsVisible { get; private set; }
        public DateTimeOffset? VisibleFrom { get; private set; }
        public DateTimeOffset? VisibleTo { get; private set; }
        public bool? IsDeleted { get; private set; }
        public string SuppliersIDs { get; private set; } = String.Empty;
        public string ParentsIDs { get; private set; } = String.Empty;
        public DateTimeOffset? CreatedDate { get; private set; }
        public string CreatedBy { get; private set; } = String.Empty;
        public DateTimeOffset? ModifiedDate { get; private set; }
        public string ModifiedBy { get; private set; } = String.Empty;

        [SkipProperty]
        public List<SkuMetadataValue> metadataValues { get; private set; } = [];

        [SkipProperty]
        public List<SkuParentRelation> parentRelations { get; private set; } = [];

        public Sku() { }

        public Sku(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Catalog.Sku dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            ProductID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.ProductID);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 400);
            SKU = ConverterValidatorsExtensions.StringLengthValidation(dto.SKU, 100);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
            UPC = ConverterValidatorsExtensions.StringLengthValidation(dto.UPC, 50);
            DisplayCondition = NullableConverters.ToNullableBoolean(dto.DisplayCondition);
            DefinitionID = NullableConverters.ToNullableInt32(dto.DefinitionID);
            ConditionID = NullableConverters.ToNullableInt32(dto.ConditionID);
            UnitOfMeasureID = NullableConverters.ToNullableInt32(dto.UnitOfMeasureID);
            PurchasingPolicyID = NullableConverters.ToNullableInt32(dto.PurchasingPolicyID);
            ManageStock = NullableConverters.ToNullableBoolean(dto.ManageStock);
            MinimumQtyAllowed = NullableConverters.ToNullableDecimal(dto.MinimumQtyAllowed);
            MaximumQtyAllowed = NullableConverters.ToNullableDecimal(dto.MaximumQtyAllowed);
            Preorderable = NullableConverters.ToNullableBoolean(dto.Preorderable);
            PreorderDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PreorderDate);
            PreorderLimit = NullableConverters.ToNullableDecimal(dto.PreorderLimit);
            Backorderable = NullableConverters.ToNullableBoolean(dto.Backorderable);
            BackorderLimit = NullableConverters.ToNullableDecimal(dto.BackorderLimit);
            WrappingQty = ConverterValidatorsExtensions.StringLengthValidation(dto.WrappingQty, 50);
            Weight = NullableConverters.ToNullableDecimal(dto.Weight);
            Height = NullableConverters.ToNullableDecimal(dto.Height);
            Width = NullableConverters.ToNullableDecimal(dto.Width);
            Depth = NullableConverters.ToNullableDecimal(dto.Depth);
            Cost = NullableConverters.ToNullableDecimal(dto.Cost);
            Tax = NullableConverters.ToNullableDecimal(dto.Tax);
            IsVisible = NullableConverters.ToNullableBoolean(dto.IsVisible);
            VisibleFrom = LinxCommerceConverters.ToNullableDateTimeOffset(dto.VisibleFrom);
            VisibleTo = LinxCommerceConverters.ToNullableDateTimeOffset(dto.VisibleTo);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
            SuppliersIDs = LinxCommerceConverters.ToText(dto.SuppliersIDs);
            ParentsIDs = LinxCommerceConverters.ToText(dto.ParentsIDs);
            CreatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedDate);
            CreatedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.CreatedBy, 100);
            ModifiedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ModifiedDate);
            ModifiedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.ModifiedBy, 100);

            // repetidos violariam a PK; sem a chave a linha também violaria
            foreach (var item in (dto.MetadataValues ?? []).Select(x => new SkuMetadataValue(doc_company, integrated_or_updated_at, ProductID, x)).Where(x => x.PropertyMetadataID != 0).GroupBy(x => x.PropertyMetadataID).Select(g => g.Last()))
                metadataValues.Add(item);
            foreach (var item in (dto.ParentRelations ?? []).Select(x => new SkuParentRelation(doc_company, integrated_or_updated_at, ProductID, x)).Where(x => x.ParentID != 0).GroupBy(x => x.ParentID).Select(g => g.Last()))
                parentRelations.Add(item);
        }
    }
}
