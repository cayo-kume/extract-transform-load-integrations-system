using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// item da composição da referência, tabela totvs.TotvsSkusCompositions (sem chave na API: a posição é a chave)
    /// </summary>
    public class Composition
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public string material { get; private set; } = String.Empty;
        public decimal? percentage { get; private set; }

        public Composition() { }

        public Composition(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, int sequence, Dtos.Reads.Products.Composition dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.sequence = sequence;
            material = ConverterValidatorsExtensions.StringLengthValidation(dto.material, 60);
            percentage = NullableConverters.ToNullableDecimal(dto.percentage);
        }
    }
}
