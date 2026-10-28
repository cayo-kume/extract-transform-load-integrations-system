namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// endereço de entrega do pedido (ShippingAddressModel)
    /// </summary>
    public class ShippingAddress
    {
        public string? cep { get; init; }           // 8 dígitos
        public string? publicPlace { get; init; }   // tipo de logradouro, até 10
        public string? address { get; init; }       // até 60
        public int? number { get; init; }           // até 5 dígitos
        public string? complement { get; init; }    // até 65
    }
}
