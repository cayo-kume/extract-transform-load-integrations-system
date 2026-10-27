namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPlanosPedidoVenda: os Planos de Pagamentos realizados nos pedidos de vendas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPlanosPedidoVenda
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_pedido { get; init; }
        public string? plano { get; init; }
        public string? desc_plano { get; init; }
        public string? total { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? indice_plano { get; init; }
        public string? valor_desc_acresc_plano { get; init; }
        public string? cod_forma_pgto { get; init; }
        public string? forma_pgto { get; init; }
        public string? timestamp { get; init; }
    }
}
