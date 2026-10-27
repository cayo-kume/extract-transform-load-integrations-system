using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// estado da região de atuação, tabela totvs.TotvsOperationalAreasStates
    /// </summary>
    public class OperationalAreaState
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int operationalAreaCode { get; private set; }
        public int stateCode { get; private set; }
        public string stateName { get; private set; } = String.Empty;

        public OperationalAreaState() { }

        public OperationalAreaState(string doc_company, DateTime integrated_or_updated_at, int operationalAreaCode, Dtos.Reads.Sellers.OperationalAreaState dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.operationalAreaCode = operationalAreaCode;

            stateCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.stateCode);
            stateName = ConverterValidatorsExtensions.StringLengthValidation(dto.stateName, 60);
        }
    }
}
