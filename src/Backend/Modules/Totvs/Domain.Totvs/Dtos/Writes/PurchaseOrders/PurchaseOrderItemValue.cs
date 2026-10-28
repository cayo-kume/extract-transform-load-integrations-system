namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// valor do item por tipo (PurchaseOrderItemValueModel): C custo ou P preço, com o código do parâmetro DS_LST_VLR_PED_COMPRA
    /// </summary>
    public class PurchaseOrderItemValue
    {
        public string? valueType { get; init; }
        public int? valueCode { get; init; }
        public decimal? value { get; init; }
    }
}
