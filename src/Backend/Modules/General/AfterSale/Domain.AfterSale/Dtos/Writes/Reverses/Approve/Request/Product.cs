namespace Domain.AfterSale.Dtos.Writes.Reverses.Approve.Request
{
    public class Product
    {
        public long? id { get; init; }
        public int? qty { get; init; }
        public string? product_received_comment { get; init; }
        public long? reason_id { get; init; }
    }
}
