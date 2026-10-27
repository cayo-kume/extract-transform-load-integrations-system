using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// CEP da região de atuação, tabela totvs.TotvsOperationalAreasCeps
    /// </summary>
    public class OperationalAreaCep
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int operationalAreaCode { get; private set; }
        public string cep { get; private set; } = String.Empty;

        public OperationalAreaCep() { }

        public OperationalAreaCep(string doc_company, DateTime integrated_or_updated_at, int operationalAreaCode, Dtos.Reads.Sellers.OperationalAreaCep dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.operationalAreaCode = operationalAreaCode;

            cep = ConverterValidatorsExtensions.StringLengthValidation(dto.cep, 10);
        }
    }
}
