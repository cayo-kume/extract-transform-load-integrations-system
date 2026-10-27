namespace Domain.Totvs.Dtos.Reads.People.Representatives
{
    /// <summary>
    /// cliente da carteira do representante (ViewRepresentativeCustomerModel)
    /// </summary>
    public class RepresentativeCustomer
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? cpfCnpj { get; init; }
    }
}
