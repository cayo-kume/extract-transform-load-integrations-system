namespace Domain.Totvs.Dtos.Reads.People.Representatives
{
    /// <summary>
    /// parâmetro de empresa que contém o representante (PersonBranchParameterModel, ADMFM014)
    /// </summary>
    public class RepresentativeParameter
    {
        public string? branchCode { get; init; }
        public string? parameterCode { get; init; }
        public string? parameterValue { get; init; }
    }
}
