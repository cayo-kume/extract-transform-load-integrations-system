namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPedidosIdentificador: os registro da tabela B2C_vendas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPedidosIdentificador
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? identificador { get; init; }
        public string? id_venda { get; init; }
        public string? order_id { get; init; }
        public string? id_cliente { get; init; }
        public string? valor_frete { get; init; }
        public string? data_origem { get; init; }
        public string? timestamp { get; init; }
    }
}
