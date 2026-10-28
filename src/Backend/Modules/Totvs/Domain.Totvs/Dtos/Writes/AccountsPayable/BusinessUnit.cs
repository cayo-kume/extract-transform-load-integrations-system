namespace Domain.Totvs.Dtos.Writes.AccountsPayable
{
    /// <summary>
    /// unidade de negócio da duplicata, com o percentual de rateio (BusinessUnitModel)
    /// </summary>
    public class BusinessUnit
    {
        public string? classificationCode { get; init; }
        public decimal? proratedPercentage { get; init; }
    }
}
