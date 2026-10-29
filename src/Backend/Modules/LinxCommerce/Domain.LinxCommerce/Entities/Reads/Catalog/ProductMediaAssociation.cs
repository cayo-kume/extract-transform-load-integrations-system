using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// associação de mídia do produto, tabela linxcommerce.LinxCommerceProductsMediaAssociations
    /// </summary>
    public class ProductMediaAssociation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public int ID { get; private set; }
        public int? MediaID { get; private set; }
        public string Path { get; private set; } = String.Empty;
        public int? Order { get; private set; }

        public ProductMediaAssociation() { }

        public ProductMediaAssociation(string doc_company, DateTime integrated_or_updated_at, int ProductID, Dtos.Reads.Catalog.ProductMediaAssociation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ProductID = ProductID;

            ID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.ID);
            MediaID = NullableConverters.ToNullableInt32(dto.MediaID);
            Path = ConverterValidatorsExtensions.StringLengthValidation(dto.Path, 1000);
            Order = NullableConverters.ToNullableInt32(dto.Order);
        }
    }
}
