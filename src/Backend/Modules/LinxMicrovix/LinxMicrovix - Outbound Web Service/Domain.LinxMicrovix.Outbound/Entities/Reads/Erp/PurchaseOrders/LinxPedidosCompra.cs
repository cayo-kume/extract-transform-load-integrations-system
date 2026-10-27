using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// LinxPedidosCompra, tabela linxmicrovix.LinxPedidosCompra. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao
    /// </summary>
    public class LinxPedidosCompra
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? cod_pedido { get; private set; }
        public DateTime? data_pedido { get; private set; }
        public int transacao { get; private set; }
        public int? usuario { get; private set; }
        public int? codigo_fornecedor { get; private set; }
        public long? cod_produto { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? valor_unitario { get; private set; }
        public int? cod_comprador { get; private set; }
        public decimal? valor_frete { get; private set; }
        public decimal? valor_total { get; private set; }
        public int? cod_plano_pagamento { get; private set; }
        public string? plano_pagamento { get; private set; }
        public string? obs { get; private set; }
        public string? aprovado { get; private set; }
        public string? cancelado { get; private set; }
        public string? encerrado { get; private set; }
        public DateTime? data_aprovacao { get; private set; }
        public string? numero_ped_fornec { get; private set; }
        public string? tipo_frete { get; private set; }
        public string? natureza_operacao { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public string? numero_projeto_officina { get; private set; }
        public string? status_pedido { get; private set; }
        public decimal? qtde_entregue { get; private set; }
        public string? descricao_frete { get; private set; }
        public bool? integrado_linx { get; private set; }
        public int? nf_gerada { get; private set; }
        public long timestamp { get; private set; }
        public int? empresa { get; private set; }
        public string? nf_origem_ws { get; private set; }
        public int? id_compras_pedido_status { get; private set; }
        public decimal? qtde_desejada { get; private set; }
        public string? motivo_ajuste { get; private set; }
        public decimal? saldo_disponivel_api { get; private set; }

        public LinxPedidosCompra() { }

        public LinxPedidosCompra(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PurchaseOrders.LinxPedidosCompra dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_pedido = MicrovixConverters.ToNullableInt32(dto.cod_pedido);
            data_pedido = MicrovixConverters.ToNullableDateTime(dto.data_pedido);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            codigo_fornecedor = MicrovixConverters.ToNullableInt32(dto.codigo_fornecedor);
            cod_produto = MicrovixConverters.ToNullableInt64(dto.cod_produto);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            valor_unitario = MicrovixConverters.ToNullableDecimal(dto.valor_unitario);
            cod_comprador = MicrovixConverters.ToNullableInt32(dto.cod_comprador);
            valor_frete = MicrovixConverters.ToNullableDecimal(dto.valor_frete);
            valor_total = MicrovixConverters.ToNullableDecimal(dto.valor_total);
            cod_plano_pagamento = MicrovixConverters.ToNullableInt32(dto.cod_plano_pagamento);
            plano_pagamento = MicrovixConverters.ToNullableString(dto.plano_pagamento, 100);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            aprovado = MicrovixConverters.ToNullableString(dto.aprovado, 1);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            encerrado = MicrovixConverters.ToNullableString(dto.encerrado, 1);
            data_aprovacao = MicrovixConverters.ToNullableDateTime(dto.data_aprovacao);
            numero_ped_fornec = MicrovixConverters.ToNullableString(dto.numero_ped_fornec, 15);
            tipo_frete = MicrovixConverters.ToNullableString(dto.tipo_frete, 1);
            natureza_operacao = MicrovixConverters.ToNullableString(dto.natureza_operacao, 73);
            previsao_entrega = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega);
            numero_projeto_officina = MicrovixConverters.ToNullableString(dto.numero_projeto_officina, 50);
            status_pedido = MicrovixConverters.ToNullableString(dto.status_pedido, 1);
            qtde_entregue = MicrovixConverters.ToNullableDecimal(dto.qtde_entregue);
            descricao_frete = MicrovixConverters.ToNullableString(dto.descricao_frete, 40);
            integrado_linx = MicrovixConverters.ToNullableBoolean(dto.integrado_linx);
            nf_gerada = MicrovixConverters.ToNullableInt32(dto.nf_gerada);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            nf_origem_ws = MicrovixConverters.ToNullableString(dto.nf_origem_ws, 30);
            id_compras_pedido_status = MicrovixConverters.ToNullableInt32(dto.id_compras_pedido_status);
            qtde_desejada = MicrovixConverters.ToNullableDecimal(dto.qtde_desejada);
            motivo_ajuste = MicrovixConverters.ToNullableString(dto.motivo_ajuste, 500);
            saldo_disponivel_api = MicrovixConverters.ToNullableDecimal(dto.saldo_disponivel_api);
        }
    }
}
