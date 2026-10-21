using Domain.AfterSale.Dtos.Writes.Base;

namespace Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Response
{
    /// <summary>
    /// response of PUT v3/api/reverses/{id}/receive-products
    /// </summary>
    public class Root : ResponseRoot
    {
        public Status? status { get; init; }
    }
}
