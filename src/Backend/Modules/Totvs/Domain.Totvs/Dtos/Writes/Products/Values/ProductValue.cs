namespace Domain.Totvs.Dtos.Writes.Products.Values
{
    /// <summary>
    /// valor de um produto numa empresa (ProductValueInsertModel): valueType C (custo) ou P (preço), como no pedido de compra; valueCode é o
    /// tipo de preço ou de custo do PRDFL003
    /// </summary>
    public class ProductValue
    {
        public int? branchCode { get; init; }
        public string? valueType { get; init; }
        public int? valueCode { get; init; }
        public decimal? value { get; init; }
    }
}
