using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Product
{
    public class Product
    {
        [SkipProperty]
        public List<Midia> Medias { get; set; } = new List<Midia>();

        [SkipProperty]
        public List<MetadataValue> MetadataValues { get; set; } = new List<MetadataValue>();

        [SkipProperty]
        public List<MediaAssociation> MediaAssociations { get; set; } = new List<MediaAssociation>();

        [SkipProperty]
        public List<int> TagsID { get; set; }

        [SkipProperty]
        public List<int> SkusID { get; set; }

        [SkipProperty]
        public List<int> ShippingRegionsID { get; set; }

        [SkipProperty]
        public List<int> FlagsID { get; set; }

        [SkipProperty]
        public List<int> MediasID { get; set; }

        [SkipProperty]
        public List<int> CategoriesID { get; set; }

        [SkipProperty]
        public Dictionary<int?, string> Responses { get; set; } = new Dictionary<int?, string>();

        public DateTime lastupdateon { get; set; }
        public Int32? BrandID { get; set; }
        public string? SkusIDs { get; set; }
        public string? FlagsIDs { get; set; }
        public Int32? RatingSetID { get; set; }
        public string? MediasIDs { get; set; }
        public string? CategoriesIDs { get; set; }
        public Int32? CatalogItemType { get; set; }
        public Int32? PurchasingPolicyID { get; set; }
        public bool? IsFreeShipping { get; set; }
        public string? ShippingRegionsIDs { get; set; }
        public string? SearchKeywords { get; set; }
        public string? PageTitle { get; set; }
        public string? UrlFriendly { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? WarrantyDescription { get; set; }
        public string? TagsIDs { get; set; }
        public bool? IsVisible { get; set; }
        public DateTimeOffset? VisibleFrom { get; set; }
        public DateTimeOffset? VisibleTo { get; set; }
        public bool? IsSearchable { get; set; }
        public bool? IsUponRequest { get; set; }
        public string? DisplayAvailability { get; set; }
        public bool? DisplayStockQty { get; set; }
        public string? DisplayPrice { get; set; }
        public bool? IsNew { get; set; }
        public DateTimeOffset? NewFrom { get; set; }
        public DateTimeOffset? NewTo { get; set; }
        public bool? IsGift { get; set; }
        public bool? UseAcceptanceTerm { get; set; }
        public Int32? AcceptanceTermID { get; set; }
        public Int32? DefinitionID { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SendToMarketplace { get; set; }
        public Int32? ProductID { get; set; }
        public string? Name { get; set; }
        public string? SKU { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? IntegrationID { get; set; }

        public Product() { }

        public Product(Product product, string? getProductResponse)
        {
            try
            {
                lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                this.BrandID = product.BrandID;
                this.SkusIDs = product.SkusIDs;
                this.FlagsIDs = product.FlagsIDs;
                this.RatingSetID = product.RatingSetID;
                this.MediasIDs = product.MediasIDs;
                this.CategoriesIDs = product.CategoriesIDs;
                this.CatalogItemType = product.CatalogItemType;
                this.PurchasingPolicyID = product.PurchasingPolicyID;
                this.IsFreeShipping = product.IsFreeShipping;
                this.ShippingRegionsIDs = product.ShippingRegionsIDs;
                this.SearchKeywords = product.SearchKeywords;
                this.PageTitle = product.PageTitle;
                this.UrlFriendly = product.UrlFriendly;
                this.MetaDescription = product.MetaDescription;
                this.MetaKeywords = product.MetaKeywords;
                this.ShortDescription = product.ShortDescription;
                this.LongDescription = product.LongDescription;
                this.WarrantyDescription = product.WarrantyDescription;
                this.TagsIDs = product.TagsIDs;
                this.IsDeleted = product.IsDeleted;
                this.VisibleFrom = product.VisibleFrom;
                this.VisibleTo = product.VisibleTo;
                this.IsSearchable = product.IsSearchable;
                this.IsUponRequest = product.IsUponRequest;
                this.DisplayAvailability = product.DisplayAvailability;
                this.DisplayStockQty = product.DisplayStockQty;
                this.DisplayPrice = product.DisplayPrice;
                this.IsNew = product.IsNew;
                this.NewFrom = product.NewFrom;
                this.NewTo = product.NewTo;
                this.IsGift = product.IsGift;
                this.UseAcceptanceTerm = product.UseAcceptanceTerm;
                this.AcceptanceTermID = product.AcceptanceTermID;
                this.DefinitionID = product.DefinitionID;
                this.IsDeleted = product.IsDeleted;
                this.SendToMarketplace = product.SendToMarketplace;
                this.ProductID = product.ProductID;
                this.Name = product.Name;
                this.SKU = product.SKU;
                this.CreatedDate = product.CreatedDate;
                this.CreatedBy = product.CreatedBy;
                this.ModifiedBy = product.ModifiedBy;
                this.ModifiedDate = product.ModifiedDate;
                this.IntegrationID = product.IntegrationID;
                this.Responses.Add(product.ProductID, getProductResponse);

                var groupedMediaAssociations = product.Medias
                                .Where(y => y.MediaAssociations != null)
                                .SelectMany(x => x.MediaAssociations)
                                .GroupBy(y => y.ID)
                                .Select(g => g.First())
                                .ToList();

                foreach (var mediaAssociations in groupedMediaAssociations)
                {
                    this.MediaAssociations.Add(new MediaAssociation(mediaAssociations));
                }

                foreach (var midia in product.Medias)
                {
                    this.Medias.Add(new Midia(midia));
                }

                foreach (var metadataValues in product.MetadataValues)
                {
                    this.MetadataValues.Add(new MetadataValue(metadataValues, product.ProductID));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
