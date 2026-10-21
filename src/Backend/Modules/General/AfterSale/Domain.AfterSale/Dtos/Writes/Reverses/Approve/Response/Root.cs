using Domain.AfterSale.Dtos.Writes.Base;

namespace Domain.AfterSale.Dtos.Writes.Reverses.Approve.Response
{
    /// <summary>
    /// response of POST v3/api/reverses/{id}/approve
    /// </summary>
    public class Root : ResponseRoot
    {
        public Status? status { get; init; }
    }
}
