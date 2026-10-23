namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// cliente vinculado ao vendedor (CustomerDto)
    /// </summary>
    public class Customer
    {
        public string? personCode { get; init; }
        public string? document { get; init; }
        public string? name { get; init; }
    }
}
