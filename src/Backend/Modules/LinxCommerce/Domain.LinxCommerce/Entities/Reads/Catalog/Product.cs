using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// produto (POST Catalog/API.svc/web/GetProduct); as listas de ids ficam como texto, tabela linxcommerce.LinxCommerceProducts
    /// </summary>
    public class Product
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string SKU { get; private set; } = String.Empty;
        public string IntegrationID { get; private set; } = String.Empty;
        public int? BrandID { get; private set; }
        public int? RatingSetID { get; private set; }
        public int? CatalogItemType { get; private set; }
        public int? PurchasingPolicyID { get; private set; }
        public int? DefinitionID { get; private set; }
        public int? AcceptanceTermID { get; private set; }
        public bool? IsFreeShipping { get; private set; }
        public bool? IsVisible { get; private set; }
        public DateTimeOffset? VisibleFrom { get; private set; }
        public DateTimeOffset? VisibleTo { get; private set; }
        public bool? IsSearchable { get; private set; }
        public bool? IsUponRequest { get; private set; }
        public string DisplayAvailability { get; private set; } = String.Empty;
        public bool? DisplayStockQty { get; private set; }
        public string DisplayPrice { get; private set; } = String.Empty;
        public bool? IsNew { get; private set; }
        public DateTimeOffset? NewFrom { get; private set; }
        public DateTimeOffset? NewTo { get; private set; }
        public bool? IsGift { get; private set; }
        public bool? UseAcceptanceTerm { get; private set; }
        public bool? IsDeleted { get; private set; }
        public string SendToMarketplace { get; private set; } = String.Empty;
        public string PageTitle { get; private set; } = String.Empty;
        public string UrlFriendly { get; private set; } = String.Empty;
        public string SearchKeywords { get; private set; } = String.Empty;
        public string MetaDescription { get; private set; } = String.Empty;
        public string MetaKeywords { get; private set; } = String.Empty;
        public string ShortDescription { get; private set; } = String.Empty;
        public string LongDescription { get; private set; } = String.Empty;
        public string WarrantyDescription { get; private set; } = String.Empty;
        public string SkusIDs { get; private set; } = String.Empty;
        public string FlagsIDs { get; private set; } = String.Empty;
        public string MediasIDs { get; private set; } = String.Empty;
        public string CategoriesIDs { get; private set; } = String.Empty;
        public string ShippingRegionsIDs { get; private set; } = String.Empty;
        public string TagsIDs { get; private set; } = String.Empty;
        public DateTimeOffset? CreatedDate { get; private set; }
        public string CreatedBy { get; private set; } = String.Empty;
        public DateTimeOffset? ModifiedDate { get; private set; }
        public string ModifiedBy { get; private set; } = String.Empty;

        [SkipProperty]
        public List<ProductMetadataValue> metadataValues { get; private set; } = [];

        [SkipProperty]
        public List<ProductMedia> medias { get; private set; } = [];

        [SkipProperty]
        public List<ProductMediaAssociation> mediaAssociations { get; private set; } = [];

        public Product() { }

        public Product(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Catalog.Product dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            ProductID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.ProductID);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 400);
            SKU = ConverterValidatorsExtensions.StringLengthValidation(dto.SKU, 100);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
            BrandID = NullableConverters.ToNullableInt32(dto.BrandID);
            RatingSetID = NullableConverters.ToNullableInt32(dto.RatingSetID);
            CatalogItemType = NullableConverters.ToNullableInt32(dto.CatalogItemType);
            PurchasingPolicyID = NullableConverters.ToNullableInt32(dto.PurchasingPolicyID);
            DefinitionID = NullableConverters.ToNullableInt32(dto.DefinitionID);
            AcceptanceTermID = NullableConverters.ToNullableInt32(dto.AcceptanceTermID);
            IsFreeShipping = NullableConverters.ToNullableBoolean(dto.IsFreeShipping);
            IsVisible = NullableConverters.ToNullableBoolean(dto.IsVisible);
            VisibleFrom = LinxCommerceConverters.ToNullableDateTimeOffset(dto.VisibleFrom);
            VisibleTo = LinxCommerceConverters.ToNullableDateTimeOffset(dto.VisibleTo);
            IsSearchable = NullableConverters.ToNullableBoolean(dto.IsSearchable);
            IsUponRequest = NullableConverters.ToNullableBoolean(dto.IsUponRequest);
            DisplayAvailability = ConverterValidatorsExtensions.StringLengthValidation(dto.DisplayAvailability, 50);
            DisplayStockQty = NullableConverters.ToNullableBoolean(dto.DisplayStockQty);
            DisplayPrice = ConverterValidatorsExtensions.StringLengthValidation(dto.DisplayPrice, 50);
            IsNew = NullableConverters.ToNullableBoolean(dto.IsNew);
            NewFrom = LinxCommerceConverters.ToNullableDateTimeOffset(dto.NewFrom);
            NewTo = LinxCommerceConverters.ToNullableDateTimeOffset(dto.NewTo);
            IsGift = NullableConverters.ToNullableBoolean(dto.IsGift);
            UseAcceptanceTerm = NullableConverters.ToNullableBoolean(dto.UseAcceptanceTerm);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
            SendToMarketplace = ConverterValidatorsExtensions.StringLengthValidation(dto.SendToMarketplace, 50);
            PageTitle = ConverterValidatorsExtensions.StringLengthValidation(dto.PageTitle, 400);
            UrlFriendly = ConverterValidatorsExtensions.StringLengthValidation(dto.UrlFriendly, 400);
            SearchKeywords = dto.SearchKeywords ?? String.Empty;
            MetaDescription = dto.MetaDescription ?? String.Empty;
            MetaKeywords = dto.MetaKeywords ?? String.Empty;
            ShortDescription = dto.ShortDescription ?? String.Empty;
            LongDescription = dto.LongDescription ?? String.Empty;
            WarrantyDescription = dto.WarrantyDescription ?? String.Empty;
            SkusIDs = LinxCommerceConverters.ToText(dto.SkusIDs);
            FlagsIDs = LinxCommerceConverters.ToText(dto.FlagsIDs);
            MediasIDs = LinxCommerceConverters.ToText(dto.MediasIDs);
            CategoriesIDs = LinxCommerceConverters.ToText(dto.CategoriesIDs);
            ShippingRegionsIDs = LinxCommerceConverters.ToText(dto.ShippingRegionsIDs);
            TagsIDs = LinxCommerceConverters.ToText(dto.TagsIDs);
            CreatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedDate);
            CreatedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.CreatedBy, 100);
            ModifiedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ModifiedDate);
            ModifiedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.ModifiedBy, 100);

            // repetidos violariam a PK; sem a chave a linha também violaria
            foreach (var item in (dto.MetadataValues ?? []).Select(x => new ProductMetadataValue(doc_company, integrated_or_updated_at, ProductID, x)).Where(x => x.PropertyMetadataID != 0).GroupBy(x => x.PropertyMetadataID).Select(g => g.Last()))
                metadataValues.Add(item);
            foreach (var item in (dto.Medias ?? []).Select(x => new ProductMedia(doc_company, integrated_or_updated_at, ProductID, x)).Where(x => x.MediaID != 0).GroupBy(x => x.MediaID).Select(g => g.Last()))
                medias.Add(item);
            foreach (var item in (dto.MediaAssociations ?? []).Select(x => new ProductMediaAssociation(doc_company, integrated_or_updated_at, ProductID, x)).Where(x => x.ID != 0).GroupBy(x => x.ID).Select(g => g.Last()))
                mediaAssociations.Add(item);
        }
    }
}
