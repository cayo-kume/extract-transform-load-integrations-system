using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// cor da referência, tabela totvs.TotvsSkusColors (code anulável na API: nulo vira '', porque faz parte da chave)
    /// </summary>
    public class Color
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public string colorCode { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string groupName { get; private set; } = String.Empty;
        public string auxiliaryCode { get; private set; } = String.Empty;
        public string auxiliaryName { get; private set; } = String.Empty;
        public string pantoneCode { get; private set; } = String.Empty;
        public string configurableField { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Product> products { get; private set; } = [];

        [SkipProperty]
        public List<MeasurementTableOmni> measurementTableOmni { get; private set; } = [];

        public Color() { }

        public Color(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, Dtos.Reads.Products.Color dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;

            colorCode = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 60);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            groupName = ConverterValidatorsExtensions.StringLengthValidation(dto.groupName, 60);
            auxiliaryCode = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryCode, 60);
            auxiliaryName = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryName, 60);
            pantoneCode = ConverterValidatorsExtensions.StringLengthValidation(dto.pantoneCode, 60);
            configurableField = ConverterValidatorsExtensions.StringLengthValidation(dto.configurableField, 60);

            // o código do produto é único no cadastro; repetidos violariam a PK
            products.AddRange((dto.products ?? []).Select(p => new Product(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, colorCode, p)).Where(p => p.code != 0).GroupBy(p => p.code).Select(g => g.Last()));
            measurementTableOmni.AddRange((dto.measurementTableOmni ?? []).Select(m => new MeasurementTableOmni(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, colorCode, m)).GroupBy(m => m.sku).Select(g => g.Last()));
        }
    }
}
