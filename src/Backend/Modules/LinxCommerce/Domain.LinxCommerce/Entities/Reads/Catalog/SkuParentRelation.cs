using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// produto pai do SKU, tabela linxcommerce.LinxCommerceSkusParentRelations
    /// </summary>
    public class SkuParentRelation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public int ParentID { get; private set; }
        public string ParentSKU { get; private set; } = String.Empty;
        public string ParentIntegrationID { get; private set; } = String.Empty;

        public SkuParentRelation() { }

        public SkuParentRelation(string doc_company, DateTime integrated_or_updated_at, int ProductID, Dtos.Reads.Catalog.SkuParentRelation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ProductID = ProductID;

            ParentID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.ParentID);
            ParentSKU = ConverterValidatorsExtensions.StringLengthValidation(dto.ParentSKU, 100);
            ParentIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.ParentIntegrationID, 100);
        }
    }
}
