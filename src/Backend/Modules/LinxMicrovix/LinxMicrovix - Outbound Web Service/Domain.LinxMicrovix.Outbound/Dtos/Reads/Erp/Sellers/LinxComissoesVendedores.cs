namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxComissoesVendedores: as comissões do vendedores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxComissoesVendedores
    {
        public string? vendedor { get; init; }
        public string? empresa { get; init; }
        public string? data_origem { get; init; }
        public string? valor_base { get; init; }
        public string? valor_comissao { get; init; }
        public string? cancelado { get; init; }
        public string? disponivel { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
