using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxPedidosVenda, tabela linxmicrovix.LinxPedidosVenda. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao
    /// </summary>
    public class LinxPedidosVenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? cod_pedido { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public string? hora_lancamento { get; private set; }
        public int transacao { get; private set; }
        public int? usuario { get; private set; }
        public int? codigo_cliente { get; private set; }
        public long? cod_produto { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? valor_unitario { get; private set; }
        public int? cod_vendedor { get; private set; }
        public decimal? valor_frete { get; private set; }
        public decimal? valor_total { get; private set; }
        public decimal? desconto_item { get; private set; }
        public int? cod_plano_pagamento { get; private set; }
        public string? plano_pagamento { get; private set; }
        public string? obs { get; private set; }
        public string? aprovado { get; private set; }
        public string? cancelado { get; private set; }
        public DateTime? data_aprovacao { get; private set; }
        public DateTime? data_alteracao { get; private set; }
        public int? tipo_frete { get; private set; }
        public string? natureza_operacao { get; private set; }
        public int? tabela_preco { get; private set; }
        public string? nome_tabela_preco { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public int? realizado_por { get; private set; }
        public int? pontuacao_ser { get; private set; }
        public string? venda_externa { get; private set; }
        public string? nf_gerada { get; private set; }
        public string? status { get; private set; }
        public string? numero_projeto_officina { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public decimal? margem_contribuicao { get; private set; }
        public int? doc_origem { get; private set; }
        public int? posicao_item { get; private set; }
        public int? orcamento_origem { get; private set; }
        public int? transacao_origem { get; private set; }
        public long timestamp { get; private set; }
        public decimal? desconto { get; private set; }
        public int? transacao_ws { get; private set; }
        public int? empresa { get; private set; }
        public long? transportador { get; private set; }
        public int? deposito { get; private set; }
        public string? texto_agrupamento { get; private set; }
        public int? posicao { get; private set; }
        public bool? encerrado { get; private set; }

        public LinxPedidosVenda() { }

        public LinxPedidosVenda(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxPedidosVenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_pedido = MicrovixConverters.ToNullableInt32(dto.cod_pedido);
            data_lancamento = MicrovixConverters.ToNullableDateTime(dto.data_lancamento);
            hora_lancamento = MicrovixConverters.ToNullableString(dto.hora_lancamento, 5);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            codigo_cliente = MicrovixConverters.ToNullableInt32(dto.codigo_cliente);
            cod_produto = MicrovixConverters.ToNullableInt64(dto.cod_produto);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            valor_unitario = MicrovixConverters.ToNullableDecimal(dto.valor_unitario);
            cod_vendedor = MicrovixConverters.ToNullableInt32(dto.cod_vendedor);
            valor_frete = MicrovixConverters.ToNullableDecimal(dto.valor_frete);
            valor_total = MicrovixConverters.ToNullableDecimal(dto.valor_total);
            desconto_item = MicrovixConverters.ToNullableDecimal(dto.desconto_item);
            cod_plano_pagamento = MicrovixConverters.ToNullableInt32(dto.cod_plano_pagamento);
            plano_pagamento = MicrovixConverters.ToNullableString(dto.plano_pagamento, 100);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            aprovado = MicrovixConverters.ToNullableString(dto.aprovado, 1);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            data_aprovacao = MicrovixConverters.ToNullableDateTime(dto.data_aprovacao);
            data_alteracao = MicrovixConverters.ToNullableDateTime(dto.data_alteracao);
            tipo_frete = MicrovixConverters.ToNullableInt32(dto.tipo_frete);
            natureza_operacao = MicrovixConverters.ToNullableString(dto.natureza_operacao, 60);
            tabela_preco = MicrovixConverters.ToNullableInt32(dto.tabela_preco);
            nome_tabela_preco = MicrovixConverters.ToNullableString(dto.nome_tabela_preco, 50);
            previsao_entrega = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega);
            realizado_por = MicrovixConverters.ToNullableInt32(dto.realizado_por);
            pontuacao_ser = MicrovixConverters.ToNullableInt32(dto.pontuacao_ser);
            venda_externa = MicrovixConverters.ToNullableString(dto.venda_externa, 1);
            nf_gerada = String.IsNullOrEmpty(dto.nf_gerada) ? null : dto.nf_gerada;
            status = MicrovixConverters.ToNullableString(dto.status, 1);
            numero_projeto_officina = MicrovixConverters.ToNullableString(dto.numero_projeto_officina, 50);
            cod_natureza_operacao = MicrovixConverters.ToNullableString(dto.cod_natureza_operacao, 10);
            margem_contribuicao = MicrovixConverters.ToNullableDecimal(dto.margem_contribuicao);
            doc_origem = MicrovixConverters.ToNullableInt32(dto.doc_origem);
            posicao_item = MicrovixConverters.ToNullableInt32(dto.posicao_item);
            orcamento_origem = MicrovixConverters.ToNullableInt32(dto.orcamento_origem);
            transacao_origem = MicrovixConverters.ToNullableInt32(dto.transacao_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            desconto = MicrovixConverters.ToNullableDecimal(dto.desconto);
            transacao_ws = MicrovixConverters.ToNullableInt32(dto.transacao_ws);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            transportador = MicrovixConverters.ToNullableInt64(dto.transportador);
            deposito = MicrovixConverters.ToNullableInt32(dto.deposito);
            texto_agrupamento = MicrovixConverters.ToNullableString(dto.texto_agrupamento, 60);
            posicao = MicrovixConverters.ToNullableInt32(dto.posicao);
            encerrado = MicrovixConverters.ToNullableBoolean(dto.encerrado);
        }
    }
}
