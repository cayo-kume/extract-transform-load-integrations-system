namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Endereço de entrega. (ShippingAddressInDto)
    /// </summary>
    public class B2COrderShippingAddress
    {
        // Cep do endereço.
        public string? cep { get; init; }
        // Nome da cidade.
        public string? cityName { get; init; }
        // Sigla UF.
        public string? stateAbbreviation { get; init; }
        // Tipo de logradouro do endereço.
        public string? publicPlace { get; init; }
        public string? address { get; init; }
        // Número do endereço.
        public int? number { get; init; }
        // Nome da bairro.
        public string? neighborhood { get; init; }
        // Complemento do endereço.
        public string? complement { get; init; }
    }
}
