namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxB2CPedidos: os pedidos de B2C (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxB2CPedidos
    {
        public string? id_pedido { get; init; }
        public string? dt_pedido { get; init; }
        public string? cod_cliente_erp { get; init; }
        public string? cod_cliente_b2c { get; init; }
        public string? vl_frete { get; init; }
        public string? forma_pgto { get; init; }
        public string? plano_pagamento { get; init; }
        public string? anotacao { get; init; }
        public string? taxa_impressao { get; init; }
        public string? finalizado { get; init; }
        public string? valor_frete_gratis { get; init; }
        public string? tipo_frete { get; init; }
        public string? id_status { get; init; }
        public string? cod_transportador { get; init; }
        public string? tipo_cobranca_frete { get; init; }
        public string? ativo { get; init; }
        public string? empresa { get; init; }
        public string? id_tabela_preco { get; init; }
        public string? valor_credito { get; init; }
        public string? cod_vendedor { get; init; }
        public string? timestamp { get; init; }
        public string? dt_insert { get; init; }
        public string? dt_disponivel_faturamento { get; init; }
        public string? mensagem_falha_faturamento { get; init; }
        public string? portal { get; init; }
        public string? order_id { get; init; }
        public string? fulfillment_id { get; init; }
        public string? id_tipo_b2c { get; init; }
        public string? ecommerce_origem { get; init; }
        public string? marketplace_loja { get; init; }
        public string? cnpj_origem_venda { get; init; }
        public string? identificador { get; init; }
    }
}
