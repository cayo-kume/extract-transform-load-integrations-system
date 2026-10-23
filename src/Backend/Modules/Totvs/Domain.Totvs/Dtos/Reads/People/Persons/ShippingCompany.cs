namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// objeto 1:1 da pessoa (shippingCompany); vira colunas do pai com o prefixo shippingCompany
    /// </summary>
    public class ShippingCompany
    {
        public string? percentegeValue { get; init; }
        public string? kilogramsValue { get; init; }
        public string? minimunValue { get; init; }
        public string? maximumValue { get; init; }
        public string? anttCode { get; init; }
        public string? typeDeductIcms { get; init; }
    }
}
