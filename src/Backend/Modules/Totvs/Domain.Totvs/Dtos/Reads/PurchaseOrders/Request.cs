namespace Domain.Totvs.Dtos.Reads.PurchaseOrders
{
    /// <summary>
    /// solicitação de compra ligada ao pedido (requestBranchCode é o branchCode da solicitação)
    /// </summary>
    public class Request
    {
        public string? branchCode { get; init; }
        public string? requestCode { get; init; }
        public string? requestDate { get; init; }
    }
}
