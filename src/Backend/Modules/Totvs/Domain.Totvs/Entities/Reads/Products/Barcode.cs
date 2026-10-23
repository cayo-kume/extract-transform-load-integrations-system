using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// código de barras do produto (code nulo vira '', faz parte da chave), tabela totvs.TotvsProductsBarCodes
    /// </summary>
    public class Barcode
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public string code { get; private set; } = String.Empty;
        public decimal? quantity { get; private set; }
        public bool? isMainCode { get; private set; }

        public Barcode() { }

        public Barcode(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.Barcode dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 60);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            isMainCode = NullableConverters.ToNullableBoolean(dto.isMainCode);
        }
    }
}
