using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimento, tabela linxmicrovix.LinxMovimento. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao
    /// </summary>
    public class LinxMovimento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int transacao { get; private set; }
        public int? usuario { get; private set; }
        public int? documento { get; private set; }
        public string? chave_nf { get; private set; }
        public int? ecf { get; private set; }
        public string? numero_serie_ecf { get; private set; }
        public int? modelo_nf { get; private set; }
        public DateTime? data_documento { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public int? codigo_cliente { get; private set; }
        public string? serie { get; private set; }
        public string? desc_cfop { get; private set; }
        public string? id_cfop { get; private set; }
        public int? cod_vendedor { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? preco_custo { get; private set; }
        public decimal? valor_liquido { get; private set; }
        public decimal? desconto { get; private set; }
        public string? cst_icms { get; private set; }
        public string? cst_pis { get; private set; }
        public string? cst_cofins { get; private set; }
        public string? cst_ipi { get; private set; }
        public decimal? valor_icms { get; private set; }
        public decimal? aliquota_icms { get; private set; }
        public decimal? base_icms { get; private set; }
        public decimal? valor_pis { get; private set; }
        public decimal? aliquota_pis { get; private set; }
        public decimal? base_pis { get; private set; }
        public decimal? valor_cofins { get; private set; }
        public decimal? aliquota_cofins { get; private set; }
        public decimal? base_cofins { get; private set; }
        public decimal? valor_icms_st { get; private set; }
        public decimal? aliquota_icms_st { get; private set; }
        public decimal? base_icms_st { get; private set; }
        public decimal? valor_ipi { get; private set; }
        public decimal? aliquota_ipi { get; private set; }
        public decimal? base_ipi { get; private set; }
        public decimal? valor_total { get; private set; }
        public bool? forma_dinheiro { get; private set; }
        public decimal? total_dinheiro { get; private set; }
        public bool? forma_cheque { get; private set; }
        public decimal? total_cheque { get; private set; }
        public bool? forma_cartao { get; private set; }
        public decimal? total_cartao { get; private set; }
        public bool? forma_crediario { get; private set; }
        public decimal? total_crediario { get; private set; }
        public bool? forma_convenio { get; private set; }
        public decimal? total_convenio { get; private set; }
        public decimal? frete { get; private set; }
        public string? operacao { get; private set; }
        public string? tipo_transacao { get; private set; }
        public long? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? cancelado { get; private set; }
        public string? excluido { get; private set; }
        public string? soma_relatorio { get; private set; }
        public Guid? identificador { get; private set; }
        public string? deposito { get; private set; }
        public string? obs { get; private set; }
        public decimal? preco_unitario { get; private set; }
        public string? hora_lancamento { get; private set; }
        public string? natureza_operacao { get; private set; }
        public int? tabela_preco { get; private set; }
        public string? nome_tabela_preco { get; private set; }
        public int? cod_sefaz_situacao { get; private set; }
        public string? desc_sefaz_situacao { get; private set; }
        public string? protocolo_aut_nfe { get; private set; }
        public DateTime? dt_update { get; private set; }
        public bool? forma_cheque_prazo { get; private set; }
        public decimal? total_cheque_prazo { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public decimal? preco_tabela_epoca { get; private set; }
        public decimal? desconto_total_item { get; private set; }
        public string? conferido { get; private set; }
        public int? transacao_pedido_venda { get; private set; }
        public string? codigo_modelo_nf { get; private set; }
        public decimal? acrescimo { get; private set; }
        public bool? mob_checkout { get; private set; }
        public decimal? aliquota_iss { get; private set; }
        public decimal? base_iss { get; private set; }
        public int? ordem { get; private set; }
        public int? codigo_rotina_origem { get; private set; }
        public long timestamp { get; private set; }
        public decimal? troco { get; private set; }
        public int? transportador { get; private set; }
        public decimal? icms_aliquota_desonerado { get; private set; }
        public decimal? icms_valor_desonerado_item { get; private set; }
        public int? empresa { get; private set; }
        public decimal? desconto_item { get; private set; }
        public decimal? aliq_iss { get; private set; }
        public decimal? iss_base_item { get; private set; }
        public decimal? despesas { get; private set; }
        public decimal? seguro_total_item { get; private set; }
        public decimal? acrescimo_total_item { get; private set; }
        public decimal? despesas_total_item { get; private set; }
        public bool? forma_pix { get; private set; }
        public decimal? total_pix { get; private set; }
        public bool? forma_deposito_bancario { get; private set; }
        public decimal? total_deposito_bancario { get; private set; }
        public int? id_venda_produto_b2c { get; private set; }
        public bool? item_promocional { get; private set; }
        public decimal? acrescimo_item { get; private set; }
        public decimal? icms_st_antecipado_aliquota { get; private set; }
        public decimal? icms_st_antecipado_margem { get; private set; }
        public decimal? icms_st_antecipado_percentual_reducao { get; private set; }
        public decimal? icms_st_antecipado_valor_item { get; private set; }
        public decimal? icms_base_desonerado_item { get; private set; }
        public string? codigo_status_nfe { get; private set; }
        public decimal? preco_produto_epoca { get; private set; }
        public int? ficha { get; private set; }
        public int? numero_sequencial_pos { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public decimal? custo_medio_epoca { get; private set; }
        public int? conhecimento { get; private set; }

        public LinxMovimento() { }

        public LinxMovimento(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            chave_nf = MicrovixConverters.ToNullableString(dto.chave_nf, 44);
            ecf = MicrovixConverters.ToNullableInt32(dto.ecf);
            numero_serie_ecf = MicrovixConverters.ToNullableString(dto.numero_serie_ecf, 30);
            modelo_nf = MicrovixConverters.ToNullableInt32(dto.modelo_nf);
            data_documento = MicrovixConverters.ToNullableDateTime(dto.data_documento);
            data_lancamento = MicrovixConverters.ToNullableDateTime(dto.data_lancamento);
            codigo_cliente = MicrovixConverters.ToNullableInt32(dto.codigo_cliente);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            desc_cfop = MicrovixConverters.ToNullableString(dto.desc_cfop, 300);
            id_cfop = MicrovixConverters.ToNullableString(dto.id_cfop, 5);
            cod_vendedor = MicrovixConverters.ToNullableInt32(dto.cod_vendedor);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            preco_custo = MicrovixConverters.ToNullableDecimal(dto.preco_custo);
            valor_liquido = MicrovixConverters.ToNullableDecimal(dto.valor_liquido);
            desconto = MicrovixConverters.ToNullableDecimal(dto.desconto);
            cst_icms = MicrovixConverters.ToNullableString(dto.cst_icms, 4);
            cst_pis = MicrovixConverters.ToNullableString(dto.cst_pis, 4);
            cst_cofins = MicrovixConverters.ToNullableString(dto.cst_cofins, 4);
            cst_ipi = MicrovixConverters.ToNullableString(dto.cst_ipi, 4);
            valor_icms = MicrovixConverters.ToNullableDecimal(dto.valor_icms);
            aliquota_icms = MicrovixConverters.ToNullableDecimal(dto.aliquota_icms);
            base_icms = MicrovixConverters.ToNullableDecimal(dto.base_icms);
            valor_pis = MicrovixConverters.ToNullableDecimal(dto.valor_pis);
            aliquota_pis = MicrovixConverters.ToNullableDecimal(dto.aliquota_pis);
            base_pis = MicrovixConverters.ToNullableDecimal(dto.base_pis);
            valor_cofins = MicrovixConverters.ToNullableDecimal(dto.valor_cofins);
            aliquota_cofins = MicrovixConverters.ToNullableDecimal(dto.aliquota_cofins);
            base_cofins = MicrovixConverters.ToNullableDecimal(dto.base_cofins);
            valor_icms_st = MicrovixConverters.ToNullableDecimal(dto.valor_icms_st);
            aliquota_icms_st = MicrovixConverters.ToNullableDecimal(dto.aliquota_icms_st);
            base_icms_st = MicrovixConverters.ToNullableDecimal(dto.base_icms_st);
            valor_ipi = MicrovixConverters.ToNullableDecimal(dto.valor_ipi);
            aliquota_ipi = MicrovixConverters.ToNullableDecimal(dto.aliquota_ipi);
            base_ipi = MicrovixConverters.ToNullableDecimal(dto.base_ipi);
            valor_total = MicrovixConverters.ToNullableDecimal(dto.valor_total);
            forma_dinheiro = MicrovixConverters.ToNullableBoolean(dto.forma_dinheiro);
            total_dinheiro = MicrovixConverters.ToNullableDecimal(dto.total_dinheiro);
            forma_cheque = MicrovixConverters.ToNullableBoolean(dto.forma_cheque);
            total_cheque = MicrovixConverters.ToNullableDecimal(dto.total_cheque);
            forma_cartao = MicrovixConverters.ToNullableBoolean(dto.forma_cartao);
            total_cartao = MicrovixConverters.ToNullableDecimal(dto.total_cartao);
            forma_crediario = MicrovixConverters.ToNullableBoolean(dto.forma_crediario);
            total_crediario = MicrovixConverters.ToNullableDecimal(dto.total_crediario);
            forma_convenio = MicrovixConverters.ToNullableBoolean(dto.forma_convenio);
            total_convenio = MicrovixConverters.ToNullableDecimal(dto.total_convenio);
            frete = MicrovixConverters.ToNullableDecimal(dto.frete);
            operacao = MicrovixConverters.ToNullableString(dto.operacao, 2);
            tipo_transacao = MicrovixConverters.ToNullableString(dto.tipo_transacao, 1);
            cod_produto = MicrovixConverters.ToNullableInt64(dto.cod_produto);
            cod_barra = MicrovixConverters.ToNullableString(dto.cod_barra, 20);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            excluido = MicrovixConverters.ToNullableString(dto.excluido, 1);
            soma_relatorio = MicrovixConverters.ToNullableString(dto.soma_relatorio, 1);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);   //fora da documentação
            deposito = MicrovixConverters.ToNullableString(dto.deposito, 100);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            preco_unitario = MicrovixConverters.ToNullableDecimal(dto.preco_unitario);
            hora_lancamento = MicrovixConverters.ToNullableString(dto.hora_lancamento, 5);
            natureza_operacao = MicrovixConverters.ToNullableString(dto.natureza_operacao, 60);
            tabela_preco = MicrovixConverters.ToNullableInt32(dto.tabela_preco);
            nome_tabela_preco = MicrovixConverters.ToNullableString(dto.nome_tabela_preco, 50);
            cod_sefaz_situacao = MicrovixConverters.ToNullableInt32(dto.cod_sefaz_situacao);
            desc_sefaz_situacao = MicrovixConverters.ToNullableString(dto.desc_sefaz_situacao, 30);
            protocolo_aut_nfe = MicrovixConverters.ToNullableString(dto.protocolo_aut_nfe, 20);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            forma_cheque_prazo = MicrovixConverters.ToNullableBoolean(dto.forma_cheque_prazo);
            total_cheque_prazo = MicrovixConverters.ToNullableDecimal(dto.total_cheque_prazo);
            cod_natureza_operacao = MicrovixConverters.ToNullableString(dto.cod_natureza_operacao, 10);
            preco_tabela_epoca = MicrovixConverters.ToNullableDecimal(dto.preco_tabela_epoca);
            desconto_total_item = MicrovixConverters.ToNullableDecimal(dto.desconto_total_item);
            conferido = MicrovixConverters.ToNullableString(dto.conferido, 1);
            transacao_pedido_venda = MicrovixConverters.ToNullableInt32(dto.transacao_pedido_venda);
            codigo_modelo_nf = MicrovixConverters.ToNullableString(dto.codigo_modelo_nf, 5);
            acrescimo = MicrovixConverters.ToNullableDecimal(dto.acrescimo);
            mob_checkout = MicrovixConverters.ToNullableBoolean(dto.mob_checkout);
            aliquota_iss = MicrovixConverters.ToNullableDecimal(dto.aliquota_iss);
            base_iss = MicrovixConverters.ToNullableDecimal(dto.base_iss);
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            codigo_rotina_origem = MicrovixConverters.ToNullableInt32(dto.codigo_rotina_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            troco = MicrovixConverters.ToNullableDecimal(dto.troco);
            transportador = MicrovixConverters.ToNullableInt32(dto.transportador);
            icms_aliquota_desonerado = MicrovixConverters.ToNullableDecimal(dto.icms_aliquota_desonerado);
            icms_valor_desonerado_item = MicrovixConverters.ToNullableDecimal(dto.icms_valor_desonerado_item);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            desconto_item = MicrovixConverters.ToNullableDecimal(dto.desconto_item);
            aliq_iss = MicrovixConverters.ToNullableDecimal(dto.aliq_iss);
            iss_base_item = MicrovixConverters.ToNullableDecimal(dto.iss_base_item);
            despesas = MicrovixConverters.ToNullableDecimal(dto.despesas);
            seguro_total_item = MicrovixConverters.ToNullableDecimal(dto.seguro_total_item);
            acrescimo_total_item = MicrovixConverters.ToNullableDecimal(dto.acrescimo_total_item);
            despesas_total_item = MicrovixConverters.ToNullableDecimal(dto.despesas_total_item);
            forma_pix = MicrovixConverters.ToNullableBoolean(dto.forma_pix);
            total_pix = MicrovixConverters.ToNullableDecimal(dto.total_pix);
            forma_deposito_bancario = MicrovixConverters.ToNullableBoolean(dto.forma_deposito_bancario);
            total_deposito_bancario = MicrovixConverters.ToNullableDecimal(dto.total_deposito_bancario);
            id_venda_produto_b2c = MicrovixConverters.ToNullableInt32(dto.id_venda_produto_b2c);
            item_promocional = MicrovixConverters.ToNullableBoolean(dto.item_promocional);
            acrescimo_item = MicrovixConverters.ToNullableDecimal(dto.acrescimo_item);
            icms_st_antecipado_aliquota = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_aliquota);
            icms_st_antecipado_margem = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_margem);
            icms_st_antecipado_percentual_reducao = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_percentual_reducao);
            icms_st_antecipado_valor_item = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_valor_item);
            icms_base_desonerado_item = MicrovixConverters.ToNullableDecimal(dto.icms_base_desonerado_item);
            codigo_status_nfe = MicrovixConverters.ToNullableString(dto.codigo_status_nfe, 5);
            preco_produto_epoca = MicrovixConverters.ToNullableDecimal(dto.preco_produto_epoca);
            ficha = MicrovixConverters.ToNullableInt32(dto.ficha);
            numero_sequencial_pos = MicrovixConverters.ToNullableInt32(dto.numero_sequencial_pos);
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            custo_medio_epoca = MicrovixConverters.ToNullableDecimal(dto.custo_medio_epoca);
            conhecimento = MicrovixConverters.ToNullableInt32(dto.conhecimento);
        }
    }
}
