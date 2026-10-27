using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// configuração de agrupador de produto (GET product/v2/product-grouper-configuration, ProductGrouperConfigurationResultModel), tabela totvs.TotvsProductGrouperConfigurations e filhas
    /// </summary>
    public class ProductGrouperConfiguration
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int grouperCode { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string type { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<ProductGrouperConfigurationItem> items { get; private set; } = [];

        public ProductGrouperConfiguration() { }

        public ProductGrouperConfiguration(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.ProductGrouperConfiguration dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            grouperCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.grouperCode);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 30);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK
            items.AddRange((dto.productGrouperItemConfigurationList ?? []).Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).Select(x => new ProductGrouperConfigurationItem(doc_company, integrated_or_updated_at, grouperCode, x)).GroupBy(x => x.code).Select(g => g.Last()));
        }
    }
}
