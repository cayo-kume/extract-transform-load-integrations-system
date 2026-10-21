namespace Domain.AfterSale.Dtos.Writes.Reverses.Approve.Request
{
    /// <summary>
    /// body of POST v3/api/reverses/{id}/approve
    /// </summary>
    public class Root
    {
        public string? comments { get; init; }
        public bool? skip_process_step { get; init; }
        public List<Product>? products { get; init; }
    }
}
