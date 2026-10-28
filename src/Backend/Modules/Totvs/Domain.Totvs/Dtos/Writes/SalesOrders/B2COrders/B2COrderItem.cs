namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Item. (ItemInDto)
    /// </summary>
    public class B2COrderItem
    {
        // Código interno do produto.
        public int? productCode { get; init; }
        // Código de identificação única do produto.
        public string? productSku { get; init; }
        // Quantidade solicitada.
        public decimal? quantity { get; init; }
        // Preço unitário original.
        public decimal? originalPrice { get; init; }
        // Preço unitário.
        public decimal? price { get; init; }
        // Percentual de desconto sobre o valor unitário.
        public decimal? discountPercentage { get; init; }
        // Valor total de desconto sobre o item.
        public decimal? discountValue { get; init; }
        // Data de previsão de faturamento.
        public DateTime? billingForecastDate { get; init; }
        // Data de início faturamento.
        public DateTime? billingStartDate { get; init; }
        // Identificador do pedido pelo utilizado pelo cliente.Campo "Número pedido cliente" do componente PEDFM069.
        public int? customerOrderCode { get; init; }
        // Identificador do produto utilizado pelo cliente.Campo "Produto cliente" do componente PEDFM069.
        public string? customerProductCode { get; init; }
        // Medida de rolo.
        public int? rollMeasureCode { get; init; }
        // Lista de códigos identificadores serial.
        public List<string>? sequentials { get; init; }
        // Lista de códigos identificadores serial.
        public List<B2COrderItemSequential>? sequentialsValues { get; init; }
        // Lista de observações do grupo do produto.
        public List<B2COrderItemGroupObservation>? groupObservations { get; init; }
        // Lista de operação triangular com quantidade.
        public List<B2COrderItemOutsourced>? outsourceds { get; init; }
        // Lista de comissionado.
        public List<B2COrderItemCommissioned>? commissioneds { get; init; }
    }
}
