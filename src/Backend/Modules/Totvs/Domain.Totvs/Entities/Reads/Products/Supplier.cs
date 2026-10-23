using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// fornecedor do produto, tabela totvs.TotvsProductsSuppliers
    /// </summary>
    public class Supplier
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string originalCode { get; private set; } = String.Empty;
        public bool? isDefault { get; private set; }

        public Supplier() { }

        public Supplier(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.Supplier dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 250);
            originalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.originalCode, 60);
            isDefault = NullableConverters.ToNullableBoolean(dto.isDefault);
        }
    }
}
