namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// endereço da pessoa
    /// </summary>
    public class Address
    {
        // ex.: Comercial, Residencial
        public string? addressType { get; init; }

        public string? country { get; init; }

        public string? postalCode { get; init; }

        public string? state { get; init; }

        public string? city { get; init; }

        public string? district { get; init; }

        public string? street { get; init; }

        public string? number { get; init; }

        public string? complement { get; init; }

        public string? reference { get; init; }

        // só um endereço principal
        public bool? isDefault { get; init; }
    }
}
