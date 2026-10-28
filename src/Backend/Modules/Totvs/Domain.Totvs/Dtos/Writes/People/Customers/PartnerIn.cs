namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// sócio da pessoa jurídica (PartnerInDto): pelo CPF/CNPJ ou pelo código da pessoa
    /// </summary>
    public class PartnerIn
    {
        public string? cpfPartner { get; init; }
        public int? partnerCode { get; init; }
        public decimal? percentageParticipation { get; init; }
    }
}
