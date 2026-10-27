using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Representatives
{
    /// <summary>
    /// parâmetro de empresa que contém o representante (PersonBranchParameterModel, ADMFM014), tabela totvs.TotvsRepresentativesParameters. personCode é o código do representante (uma pessoa do PESFM010)
    /// </summary>
    public class RepresentativeParameter
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int branchCode { get; private set; }
        public string parameterCode { get; private set; } = String.Empty;
        public string parameterValue { get; private set; } = String.Empty;

        public RepresentativeParameter() { }

        public RepresentativeParameter(string doc_company, DateTime integrated_or_updated_at, int personCode, Dtos.Reads.People.Representatives.RepresentativeParameter dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            parameterCode = ConverterValidatorsExtensions.StringLengthValidation(dto.parameterCode, 25);
            parameterValue = ConverterValidatorsExtensions.StringLengthValidation(dto.parameterValue, 2000);
        }
    }
}
