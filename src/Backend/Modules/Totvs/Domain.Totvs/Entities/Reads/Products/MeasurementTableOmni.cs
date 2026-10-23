using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// tabela de medidas omni da cor, tabela totvs.TotvsSkusColorsMeasurementsTableOmni (sku nulo vira '', faz parte da chave)
    /// </summary>
    public class MeasurementTableOmni
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public string colorCode { get; private set; } = String.Empty;
        public string sku { get; private set; } = String.Empty;
        public string tamanho { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Medida> medidas { get; private set; } = [];

        public MeasurementTableOmni() { }

        public MeasurementTableOmni(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, string colorCode, Dtos.Reads.Products.MeasurementTableOmni dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.colorCode = colorCode;

            sku = ConverterValidatorsExtensions.StringLengthValidation(dto.sku, 60);
            tamanho = ConverterValidatorsExtensions.StringLengthValidation(dto.tamanho, 60);

            medidas.AddRange((dto.medidas ?? []).Select(m => new Medida(doc_company, integrated_or_updated_at, branchInfoCode, referenceId, colorCode, sku, m)).GroupBy(m => m.chave).Select(g => g.Last()));
        }
    }
}
