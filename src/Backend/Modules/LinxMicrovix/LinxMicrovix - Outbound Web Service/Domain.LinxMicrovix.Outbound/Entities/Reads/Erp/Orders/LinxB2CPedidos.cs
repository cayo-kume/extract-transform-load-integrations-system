using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxB2CPedidos, tabela linxmicrovix.LinxB2CPedidos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_pedido
    /// </summary>
    public class LinxB2CPedidos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_pedido { get; private set; }
        public DateTime? dt_pedido { get; private set; }
        public int? cod_cliente_erp { get; private set; }
        public int? cod_cliente_b2c { get; private set; }
        public decimal? vl_frete { get; private set; }
        public int? forma_pgto { get; private set; }
        public int? plano_pagamento { get; private set; }
        public string? anotacao { get; private set; }
        public decimal? taxa_impressao { get; private set; }
        public bool? finalizado { get; private set; }
        public decimal? valor_frete_gratis { get; private set; }
        public int? tipo_frete { get; private set; }
        public int? id_status { get; private set; }
        public int? cod_transportador { get; private set; }
        public int? tipo_cobranca_frete { get; private set; }
        public bool? ativo { get; private set; }
        public int? empresa { get; private set; }
        public int? id_tabela_preco { get; private set; }
        public decimal? valor_credito { get; private set; }
        public int? cod_vendedor { get; private set; }
        public long timestamp { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public DateTime? dt_disponivel_faturamento { get; private set; }
        public string? mensagem_falha_faturamento { get; private set; }
        public int? portal { get; private set; }
        public string? order_id { get; private set; }
        public string? fulfillment_id { get; private set; }
        public int? id_tipo_b2c { get; private set; }
        public string? ecommerce_origem { get; private set; }
        public string? marketplace_loja { get; private set; }
        public string? cnpj_origem_venda { get; private set; }
        public Guid? identificador { get; private set; }

        public LinxB2CPedidos() { }

        public LinxB2CPedidos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxB2CPedidos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_pedido = MicrovixConverters.ToInt32(dto.id_pedido);
            dt_pedido = MicrovixConverters.ToNullableDateTime(dto.dt_pedido);
            cod_cliente_erp = MicrovixConverters.ToNullableInt32(dto.cod_cliente_erp);
            cod_cliente_b2c = MicrovixConverters.ToNullableInt32(dto.cod_cliente_b2c);
            vl_frete = MicrovixConverters.ToNullableDecimal(dto.vl_frete);
            forma_pgto = MicrovixConverters.ToNullableInt32(dto.forma_pgto);
            plano_pagamento = MicrovixConverters.ToNullableInt32(dto.plano_pagamento);
            anotacao = MicrovixConverters.ToNullableString(dto.anotacao, 400);
            taxa_impressao = MicrovixConverters.ToNullableDecimal(dto.taxa_impressao);
            finalizado = MicrovixConverters.ToNullableBoolean(dto.finalizado);
            valor_frete_gratis = MicrovixConverters.ToNullableDecimal(dto.valor_frete_gratis);
            tipo_frete = MicrovixConverters.ToNullableInt32(dto.tipo_frete);
            id_status = MicrovixConverters.ToNullableInt32(dto.id_status);
            cod_transportador = MicrovixConverters.ToNullableInt32(dto.cod_transportador);
            tipo_cobranca_frete = MicrovixConverters.ToNullableInt32(dto.tipo_cobranca_frete);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_tabela_preco = MicrovixConverters.ToNullableInt32(dto.id_tabela_preco);
            valor_credito = MicrovixConverters.ToNullableDecimal(dto.valor_credito);
            cod_vendedor = MicrovixConverters.ToNullableInt32(dto.cod_vendedor);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            dt_disponivel_faturamento = MicrovixConverters.ToNullableDateTime(dto.dt_disponivel_faturamento);
            mensagem_falha_faturamento = String.IsNullOrEmpty(dto.mensagem_falha_faturamento) ? null : dto.mensagem_falha_faturamento;
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            order_id = MicrovixConverters.ToNullableString(dto.order_id, 40);
            fulfillment_id = MicrovixConverters.ToNullableString(dto.fulfillment_id, 40);
            id_tipo_b2c = MicrovixConverters.ToNullableInt32(dto.id_tipo_b2c);
            ecommerce_origem = MicrovixConverters.ToNullableString(dto.ecommerce_origem, 200);
            marketplace_loja = MicrovixConverters.ToNullableString(dto.marketplace_loja, 60);
            cnpj_origem_venda = MicrovixConverters.ToNullableString(dto.cnpj_origem_venda, 14);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
        }
    }
}
