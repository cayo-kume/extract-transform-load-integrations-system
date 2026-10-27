using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// item da configuração de agrupador (ProductGrouperItemConfigurationModel), tabela totvs.TotvsProductGrouperConfigurationsItems
    /// </summary>
    public class ProductGrouperConfigurationItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int grouperCode { get; private set; }
        public int code { get; private set; }
        public int? order { get; private set; }
        public string description { get; private set; } = String.Empty;
        public bool? isMandatory { get; private set; }

        public ProductGrouperConfigurationItem() { }

        public ProductGrouperConfigurationItem(string doc_company, DateTime integrated_or_updated_at, int grouperCode, Dtos.Reads.Products.Catalogs.ProductGrouperConfigurationItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.grouperCode = grouperCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            order = NullableConverters.ToNullableInt32(dto.order);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 40);
            isMandatory = NullableConverters.ToNullableBoolean(dto.isMandatory);
        }
    }
}
