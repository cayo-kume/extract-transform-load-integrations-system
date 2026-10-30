using Newtonsoft.Json.Linq;

namespace Domain.LinxCommerce.Dtos.Reads.Catalog
{
    /// <summary>
    /// Product do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class Product
    {
        public string? ProductID { get; init; }
        public string? Name { get; init; }
        public string? SKU { get; init; }
        public string? IntegrationID { get; init; }
        public string? BrandID { get; init; }
        public string? RatingSetID { get; init; }
        public string? CatalogItemType { get; init; }
        public string? PurchasingPolicyID { get; init; }
        public string? IsFreeShipping { get; init; }
        public string? SearchKeywords { get; init; }
        public string? PageTitle { get; init; }
        public string? UrlFriendly { get; init; }
        public string? MetaDescription { get; init; }
        public string? MetaKeywords { get; init; }
        public string? ShortDescription { get; init; }
        public string? LongDescription { get; init; }
        public string? WarrantyDescription { get; init; }
        public string? IsVisible { get; init; }
        public string? VisibleFrom { get; init; }
        public string? VisibleTo { get; init; }
        public string? IsSearchable { get; init; }
        public string? IsUponRequest { get; init; }
        public string? DisplayAvailability { get; init; }
        public string? DisplayStockQty { get; init; }
        public string? DisplayPrice { get; init; }
        public string? IsNew { get; init; }
        public string? NewFrom { get; init; }
        public string? NewTo { get; init; }
        public string? IsGift { get; init; }
        public string? UseAcceptanceTerm { get; init; }
        public string? AcceptanceTermID { get; init; }
        public string? DefinitionID { get; init; }
        public string? IsDeleted { get; init; }
        public string? SendToMarketplace { get; init; }
        public string? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public string? ModifiedDate { get; init; }
        public string? ModifiedBy { get; init; }

        public JToken? SkusIDs { get; init; }
        public JToken? FlagsIDs { get; init; }
        public JToken? MediasIDs { get; init; }
        public JToken? CategoriesIDs { get; init; }
        public JToken? ShippingRegionsIDs { get; init; }
        public JToken? TagsIDs { get; init; }
        public List<ProductMetadataValue>? MetadataValues { get; init; }
        public List<ProductMedia>? Medias { get; init; }
        public List<ProductMediaAssociation>? MediaAssociations { get; init; }
    }
}
