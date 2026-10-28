namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// onde a linha de observação do pedido de compra aparece: vai como número (T3). A 2 é visível ao fornecedor
    /// </summary>
    public enum VisualizationObservationType
    {
        Internal = 1,   // interna
        Supplier = 2,   // fornecedor
        Alert = 3   // alerta
    }
}
