using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products.Catalogs
{
    /// <summary>
    /// categoria de produto (GET product/v2/category, CategoryModel), tabela totvs.TotvsCategories
    /// </summary>
    public class Category
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;
        public int? parentCategoryCode { get; private set; }
        public string categoryType { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        public Category() { }

        public Category(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Products.Catalogs.Category dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
            parentCategoryCode = NullableConverters.ToNullableInt32(dto.parentCategoryCode);
            categoryType = ConverterValidatorsExtensions.StringLengthValidation(dto.categoryType, 30);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}
