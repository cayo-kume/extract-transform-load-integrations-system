namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrdensServicoProdutos: os produtos das ordens de serviço (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrdensServicoProdutos
    {
        public string? numero_os { get; init; }
        public string? cod_produto_serv { get; init; }
        public string? id_ordservprod { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
