namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosVenda: informações de pedidos de venda de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosVenda
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_pedido { get; init; }
        public string? data_lancamento { get; init; }
        public string? hora_lancamento { get; init; }
        public string? transacao { get; init; }
        public string? usuario { get; init; }
        public string? codigo_cliente { get; init; }
        public string? cod_produto { get; init; }
        public string? quantidade { get; init; }
        public string? valor_unitario { get; init; }
        public string? cod_vendedor { get; init; }
        public string? valor_frete { get; init; }
        public string? valor_total { get; init; }
        public string? desconto_item { get; init; }
        public string? cod_plano_pagamento { get; init; }
        public string? plano_pagamento { get; init; }
        public string? obs { get; init; }
        public string? aprovado { get; init; }
        public string? cancelado { get; init; }
        public string? data_aprovacao { get; init; }
        public string? data_alteracao { get; init; }
        public string? tipo_frete { get; init; }
        public string? natureza_operacao { get; init; }
        public string? tabela_preco { get; init; }
        public string? nome_tabela_preco { get; init; }
        public string? previsao_entrega { get; init; }
        public string? realizado_por { get; init; }
        public string? pontuacao_ser { get; init; }
        public string? venda_externa { get; init; }
        public string? nf_gerada { get; init; }
        public string? status { get; init; }
        public string? numero_projeto_officina { get; init; }
        public string? cod_natureza_operacao { get; init; }
        public string? margem_contribuicao { get; init; }
        public string? doc_origem { get; init; }
        public string? posicao_item { get; init; }
        public string? orcamento_origem { get; init; }
        public string? transacao_origem { get; init; }
        public string? timestamp { get; init; }
        public string? desconto { get; init; }
        public string? transacao_ws { get; init; }
        public string? empresa { get; init; }
        public string? transportador { get; init; }
        public string? deposito { get; init; }
        public string? texto_agrupamento { get; init; }
        public string? posicao { get; init; }
        public string? encerrado { get; init; }
    }
}
