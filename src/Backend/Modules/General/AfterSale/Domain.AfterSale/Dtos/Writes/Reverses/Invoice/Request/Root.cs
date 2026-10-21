namespace Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Request
{
    /// <summary>
    /// body (application/json) of POST v3/api/reverses/{id}/invoice.
    /// which fields are required depends on the courier: see the courier attributes of the reverse.
    /// the reverse id is filled by the service from the route.
    /// </summary>
    public class Root
    {
        public string? nf_access_key { get; init; }
        public string? nf_number { get; init; }
        public string? nf_serial { get; init; }
        public decimal? nf_amount { get; init; }
        public string? nf_cfop { get; init; }
        public string? weight { get; init; }
    }
}
