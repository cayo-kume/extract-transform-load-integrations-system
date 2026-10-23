using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// medida da tabela omni, tabela totvs.TotvsSkusColorsMeasurementsTableOmniMedidas (chave nula vira '', faz parte da chave)
    /// </summary>
    public class Medida
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public string colorCode { get; private set; } = String.Empty;
        public string sku { get; private set; } = String.Empty;
        public string chave { get; private set; } = String.Empty;
        public string valor { get; private set; } = String.Empty;

        public Medida() { }

        public Medida(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, string colorCode, string sku, Dtos.Reads.Products.Medida dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.sku = sku;

            chave = ConverterValidatorsExtensions.StringLengthValidation(dto.chave, 60);
            valor = ConverterValidatorsExtensions.StringLengthValidation(dto.valor, 60);
        }
    }
}
