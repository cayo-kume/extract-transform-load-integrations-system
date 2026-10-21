namespace Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Request
{
    /// <summary>
    /// body of PUT v3/api/reverses/{id}/receive-products
    /// </summary>
    public class Root
    {
        public List<Product>? products { get; init; }
    }
}
