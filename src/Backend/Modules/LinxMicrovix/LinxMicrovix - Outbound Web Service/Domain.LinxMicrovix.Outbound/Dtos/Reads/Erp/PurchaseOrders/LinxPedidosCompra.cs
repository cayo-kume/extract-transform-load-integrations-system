namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPedidosCompra: informações de pedidos de compra de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPedidosCompra
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_pedido { get; init; }
        public string? data_pedido { get; init; }
        public string? transacao { get; init; }
        public string? usuario { get; init; }
        public string? codigo_fornecedor { get; init; }
        public string? cod_produto { get; init; }
        public string? quantidade { get; init; }
        public string? valor_unitario { get; init; }
        public string? cod_comprador { get; init; }
        public string? valor_frete { get; init; }
        public string? valor_total { get; init; }
        public string? cod_plano_pagamento { get; init; }
        public string? plano_pagamento { get; init; }
        public string? obs { get; init; }
        public string? aprovado { get; init; }
        public string? cancelado { get; init; }
        public string? encerrado { get; init; }
        public string? data_aprovacao { get; init; }
        public string? numero_ped_fornec { get; init; }
        public string? tipo_frete { get; init; }
        public string? natureza_operacao { get; init; }
        public string? previsao_entrega { get; init; }
        public string? numero_projeto_officina { get; init; }
        public string? status_pedido { get; init; }
        public string? qtde_entregue { get; init; }
        public string? descricao_frete { get; init; }
        public string? integrado_linx { get; init; }
        public string? nf_gerada { get; init; }
        public string? timestamp { get; init; }
        public string? empresa { get; init; }
        public string? nf_origem_ws { get; init; }
        public string? id_compras_pedido_status { get; init; }
        public string? qtde_desejada { get; init; }
        public string? motivo_ajuste { get; init; }
        public string? saldo_disponivel_api { get; init; }
    }
}
