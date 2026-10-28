namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// endereço (AddressInDto). Sem sequence, um endereço de entrega igual a um existente não é incluído de novo
    /// </summary>
    public class AddressIn
    {
        public string? addressType { get; init; }
        public int? sequence { get; init; }
        public int? postOfficeBox { get; init; }
        public string? cep { get; init; }
        public string? cityName { get; init; }
        public string? stateAbbreviation { get; init; }
        public string? publicPlace { get; init; }
        public string? address { get; init; }
        public int? number { get; init; }
        public string? reference { get; init; }
        public string? complement { get; init; }
        public string? neighborhood { get; init; }
    }
}
