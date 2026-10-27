using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxDadosOpticosDav, tabela linxmicrovix.LinxDadosOpticosDav. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_vendas_pos_produtos
    /// </summary>
    public class LinxDadosOpticosDav
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long id_vendas_pos_produtos { get; private set; }
        public long? cod_produto { get; private set; }
        public int? dav { get; private set; }
        public string? tipo_lente { get; private set; }
        public string? od_oe { get; private set; }
        public decimal? lng_esferico_od { get; private set; }
        public decimal? lng_cilindrico_od { get; private set; }
        public decimal? lng_eixo_od { get; private set; }
        public decimal? lng_esferico_oe { get; private set; }
        public decimal? lng_cilindrico_oe { get; private set; }
        public decimal? lng_eixo_oe { get; private set; }
        public decimal? prt_esferico_od { get; private set; }
        public decimal? prt_cilindrico_od { get; private set; }
        public decimal? prt_eixo_od { get; private set; }
        public decimal? prt_esferico_oe { get; private set; }
        public decimal? prt_cilindrico_oe { get; private set; }
        public decimal? prt_eixo_oe { get; private set; }
        public decimal? cmpl_add_od { get; private set; }
        public decimal? base_od { get; private set; }
        public decimal? diametro_od { get; private set; }
        public decimal? cmpl_add_oe { get; private set; }
        public decimal? base_oe { get; private set; }
        public decimal? diametro_oe { get; private set; }
        public decimal? ponte { get; private set; }
        public decimal? altura { get; private set; }
        public decimal? aro { get; private set; }
        public decimal? diag_maior { get; private set; }
        public decimal? dist_haste { get; private set; }
        public decimal? dist_front { get; private set; }
        public string? tipo_armacao { get; private set; }
        public decimal? curvatura { get; private set; }
        public int? id_produtos_opticos_tipo_aro { get; private set; }
        public int? id_produtos_opticos_formato_aro { get; private set; }
        public decimal? lng_dpdpn_od { get; private set; }
        public decimal? prt_dpdpn_od { get; private set; }
        public decimal? lng_dpdpn_oe { get; private set; }
        public decimal? prt_dpdpn_oe { get; private set; }
        public decimal? lng_altura_od { get; private set; }
        public decimal? prt_altura_od { get; private set; }
        public decimal? lng_altura_oe { get; private set; }
        public decimal? prt_altura_oe { get; private set; }
        public decimal? calibre { get; private set; }
        public decimal? dp_montagem { get; private set; }
        public string? observacao { get; private set; }
        public string? paciente { get; private set; }
        public string? endereco_paciente { get; private set; }
        public string? medico_especialista { get; private set; }
        public string? endereco_medico_especialista { get; private set; }
        public string? refracionista_optico { get; private set; }
        public string? data_receita { get; private set; }
        public long timestamp { get; private set; }
        public int? id_ordservprod { get; private set; }
        public int? id_vendas_pos_produtos_tmp { get; private set; }
        public long? id_vendas_pos { get; private set; }
        public bool? cancelado { get; private set; }
        public DateTime? previsao_entrega { get; private set; }
        public long? numero_ficha { get; private set; }
        public string? id_vendas_pos_produtos_campos_adicionais_tmp { get; private set; }
        public int? id_link_pagamento_linx_pay_hub { get; private set; }
        public string? codigo_gerencial { get; private set; }
        public int? empresa { get; private set; }
        public int? origem { get; private set; }
        public decimal? acoes_promocionais_desconto { get; private set; }
        public decimal? desconto { get; private set; }
        public decimal? acrescimo { get; private set; }
        public decimal? frete { get; private set; }
        public decimal? descontos_impostos { get; private set; }
        public decimal? acrescimos_impostos { get; private set; }
        public decimal? preco_unitario { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? desconto_item { get; private set; }
        public decimal? acrescimo_item_valor { get; private set; }
        public decimal? desconto_item_valor { get; private set; }
        public int? portal { get; private set; }
        public decimal? lng_adicao_od { get; private set; }
        public decimal? lng_adicao_oe { get; private set; }
        public string? grupo_servico { get; private set; }
        public decimal? lng_pris1_od { get; private set; }
        public int? lng_pris1_od_desc { get; private set; }
        public decimal? lng_pris1_oe { get; private set; }
        public int? lng_pris1_oe_desc { get; private set; }
        public decimal? prt_pris1_od { get; private set; }
        public int? prt_pris1_od_desc { get; private set; }
        public decimal? prt_pris1_oe { get; private set; }
        public int? prt_pris1_oe_desc { get; private set; }
        public decimal? lng_pris2_od { get; private set; }
        public int? lng_pris2_od_desc { get; private set; }
        public decimal? lng_pris2_oe { get; private set; }
        public int? lng_pris2_oe_desc { get; private set; }
        public decimal? prt_pris2_od { get; private set; }
        public int? prt_pris2_od_desc { get; private set; }
        public decimal? prt_pris2_oe { get; private set; }
        public int? prt_pris2_oe_desc { get; private set; }
        public decimal? dist_vertice { get; private set; }
        public decimal? inc_planto { get; private set; }
        public string? ativ_princ { get; private set; }
        public string? area_priv { get; private set; }
        public decimal? dist_visao { get; private set; }
        public string? personalizada { get; private set; }
        public decimal? pris_od { get; private set; }
        public decimal? pris_oe { get; private set; }
        public decimal? eixo_od { get; private set; }
        public decimal? eixo_oe { get; private set; }
        public decimal? borda_od { get; private set; }
        public decimal? borda_oe { get; private set; }
        public decimal? centro_od { get; private set; }
        public decimal? centro_oe { get; private set; }
        public decimal? furo_od { get; private set; }
        public decimal? furo_oe { get; private set; }
        public string? format_corte { get; private set; }
        public string? dist_leitura { get; private set; }
        public bool? dados_armacao_informados_manualmente { get; private set; }
        public string? codigo_gerencial_2 { get; private set; }
        public bool? pronta_entrega { get; private set; }
        public int? cod_cliente_medico_especialista { get; private set; }
        public int? cod_cliente_clinica { get; private set; }
        public int? vendedor { get; private set; }

        public LinxDadosOpticosDav() { }

        public LinxDadosOpticosDav(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxDadosOpticosDav dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_vendas_pos_produtos = MicrovixConverters.ToInt64(dto.id_vendas_pos_produtos);
            cod_produto = MicrovixConverters.ToNullableInt64(dto.cod_produto);
            dav = MicrovixConverters.ToNullableInt32(dto.dav);
            tipo_lente = MicrovixConverters.ToNullableString(dto.tipo_lente, 20);
            od_oe = MicrovixConverters.ToNullableString(dto.od_oe, 2);
            lng_esferico_od = MicrovixConverters.ToNullableDecimal(dto.lng_esferico_od);
            lng_cilindrico_od = MicrovixConverters.ToNullableDecimal(dto.lng_cilindrico_od);
            lng_eixo_od = MicrovixConverters.ToNullableDecimal(dto.lng_eixo_od);
            lng_esferico_oe = MicrovixConverters.ToNullableDecimal(dto.lng_esferico_oe);
            lng_cilindrico_oe = MicrovixConverters.ToNullableDecimal(dto.lng_cilindrico_oe);
            lng_eixo_oe = MicrovixConverters.ToNullableDecimal(dto.lng_eixo_oe);
            prt_esferico_od = MicrovixConverters.ToNullableDecimal(dto.prt_esferico_od);
            prt_cilindrico_od = MicrovixConverters.ToNullableDecimal(dto.prt_cilindrico_od);
            prt_eixo_od = MicrovixConverters.ToNullableDecimal(dto.prt_eixo_od);
            prt_esferico_oe = MicrovixConverters.ToNullableDecimal(dto.prt_esferico_oe);
            prt_cilindrico_oe = MicrovixConverters.ToNullableDecimal(dto.prt_cilindrico_oe);
            prt_eixo_oe = MicrovixConverters.ToNullableDecimal(dto.prt_eixo_oe);
            cmpl_add_od = MicrovixConverters.ToNullableDecimal(dto.cmpl_add_od);
            base_od = MicrovixConverters.ToNullableDecimal(dto.base_od);
            diametro_od = MicrovixConverters.ToNullableDecimal(dto.diametro_od);
            cmpl_add_oe = MicrovixConverters.ToNullableDecimal(dto.cmpl_add_oe);
            base_oe = MicrovixConverters.ToNullableDecimal(dto.base_oe);
            diametro_oe = MicrovixConverters.ToNullableDecimal(dto.diametro_oe);
            ponte = MicrovixConverters.ToNullableDecimal(dto.ponte);
            altura = MicrovixConverters.ToNullableDecimal(dto.altura);
            aro = MicrovixConverters.ToNullableDecimal(dto.aro);
            diag_maior = MicrovixConverters.ToNullableDecimal(dto.diag_maior);
            dist_haste = MicrovixConverters.ToNullableDecimal(dto.dist_haste);
            dist_front = MicrovixConverters.ToNullableDecimal(dto.dist_front);
            tipo_armacao = MicrovixConverters.ToNullableString(dto.tipo_armacao, 1);
            curvatura = MicrovixConverters.ToNullableDecimal(dto.curvatura);
            id_produtos_opticos_tipo_aro = MicrovixConverters.ToNullableInt32(dto.id_produtos_opticos_tipo_aro);
            id_produtos_opticos_formato_aro = MicrovixConverters.ToNullableInt32(dto.id_produtos_opticos_formato_aro);
            lng_dpdpn_od = MicrovixConverters.ToNullableDecimal(dto.lng_dpdpn_od);
            prt_dpdpn_od = MicrovixConverters.ToNullableDecimal(dto.prt_dpdpn_od);
            lng_dpdpn_oe = MicrovixConverters.ToNullableDecimal(dto.lng_dpdpn_oe);
            prt_dpdpn_oe = MicrovixConverters.ToNullableDecimal(dto.prt_dpdpn_oe);
            lng_altura_od = MicrovixConverters.ToNullableDecimal(dto.lng_altura_od);
            prt_altura_od = MicrovixConverters.ToNullableDecimal(dto.prt_altura_od);
            lng_altura_oe = MicrovixConverters.ToNullableDecimal(dto.lng_altura_oe);
            prt_altura_oe = MicrovixConverters.ToNullableDecimal(dto.prt_altura_oe);
            calibre = MicrovixConverters.ToNullableDecimal(dto.calibre);
            dp_montagem = MicrovixConverters.ToNullableDecimal(dto.dp_montagem);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 1000);   //fora da documentação
            paciente = MicrovixConverters.ToNullableString(dto.paciente, 100);
            endereco_paciente = MicrovixConverters.ToNullableString(dto.endereco_paciente, 100);
            medico_especialista = MicrovixConverters.ToNullableString(dto.medico_especialista, 100);
            endereco_medico_especialista = MicrovixConverters.ToNullableString(dto.endereco_medico_especialista, 100);
            refracionista_optico = MicrovixConverters.ToNullableString(dto.refracionista_optico, 100);
            data_receita = MicrovixConverters.ToNullableString(dto.data_receita, 1000);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_ordservprod = MicrovixConverters.ToNullableInt32(dto.id_ordservprod);
            id_vendas_pos_produtos_tmp = MicrovixConverters.ToNullableInt32(dto.id_vendas_pos_produtos_tmp);
            id_vendas_pos = MicrovixConverters.ToNullableInt64(dto.id_vendas_pos);
            cancelado = MicrovixConverters.ToNullableBoolean(dto.cancelado);
            previsao_entrega = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega);   //fora da documentação
            numero_ficha = MicrovixConverters.ToNullableInt64(dto.numero_ficha);
            id_vendas_pos_produtos_campos_adicionais_tmp = MicrovixConverters.ToNullableString(dto.id_vendas_pos_produtos_campos_adicionais_tmp, 1000);   //fora da documentação
            id_link_pagamento_linx_pay_hub = MicrovixConverters.ToNullableInt32(dto.id_link_pagamento_linx_pay_hub);
            codigo_gerencial = MicrovixConverters.ToNullableString(dto.codigo_gerencial, 20);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            origem = MicrovixConverters.ToNullableInt32(dto.origem);
            acoes_promocionais_desconto = MicrovixConverters.ToNullableDecimal(dto.acoes_promocionais_desconto);
            desconto = MicrovixConverters.ToNullableDecimal(dto.desconto);
            acrescimo = MicrovixConverters.ToNullableDecimal(dto.acrescimo);
            frete = MicrovixConverters.ToNullableDecimal(dto.frete);
            descontos_impostos = MicrovixConverters.ToNullableDecimal(dto.descontos_impostos);
            acrescimos_impostos = MicrovixConverters.ToNullableDecimal(dto.acrescimos_impostos);
            preco_unitario = MicrovixConverters.ToNullableDecimal(dto.preco_unitario);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            desconto_item = MicrovixConverters.ToNullableDecimal(dto.desconto_item);
            acrescimo_item_valor = MicrovixConverters.ToNullableDecimal(dto.acrescimo_item_valor);
            desconto_item_valor = MicrovixConverters.ToNullableDecimal(dto.desconto_item_valor);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            lng_adicao_od = MicrovixConverters.ToNullableDecimal(dto.lng_adicao_od);
            lng_adicao_oe = MicrovixConverters.ToNullableDecimal(dto.lng_adicao_oe);
            grupo_servico = MicrovixConverters.ToNullableString(dto.grupo_servico, 2);
            lng_pris1_od = MicrovixConverters.ToNullableDecimal(dto.lng_pris1_od);
            lng_pris1_od_desc = MicrovixConverters.ToNullableInt32(dto.lng_pris1_od_desc);
            lng_pris1_oe = MicrovixConverters.ToNullableDecimal(dto.lng_pris1_oe);
            lng_pris1_oe_desc = MicrovixConverters.ToNullableInt32(dto.lng_pris1_oe_desc);
            prt_pris1_od = MicrovixConverters.ToNullableDecimal(dto.prt_pris1_od);
            prt_pris1_od_desc = MicrovixConverters.ToNullableInt32(dto.prt_pris1_od_desc);
            prt_pris1_oe = MicrovixConverters.ToNullableDecimal(dto.prt_pris1_oe);
            prt_pris1_oe_desc = MicrovixConverters.ToNullableInt32(dto.prt_pris1_oe_desc);
            lng_pris2_od = MicrovixConverters.ToNullableDecimal(dto.lng_pris2_od);
            lng_pris2_od_desc = MicrovixConverters.ToNullableInt32(dto.lng_pris2_od_desc);
            lng_pris2_oe = MicrovixConverters.ToNullableDecimal(dto.lng_pris2_oe);
            lng_pris2_oe_desc = MicrovixConverters.ToNullableInt32(dto.lng_pris2_oe_desc);
            prt_pris2_od = MicrovixConverters.ToNullableDecimal(dto.prt_pris2_od);
            prt_pris2_od_desc = MicrovixConverters.ToNullableInt32(dto.prt_pris2_od_desc);
            prt_pris2_oe = MicrovixConverters.ToNullableDecimal(dto.prt_pris2_oe);
            prt_pris2_oe_desc = MicrovixConverters.ToNullableInt32(dto.prt_pris2_oe_desc);
            dist_vertice = MicrovixConverters.ToNullableDecimal(dto.dist_vertice);
            inc_planto = MicrovixConverters.ToNullableDecimal(dto.inc_planto);
            ativ_princ = MicrovixConverters.ToNullableString(dto.ativ_princ, 100);
            area_priv = MicrovixConverters.ToNullableString(dto.area_priv, 100);
            dist_visao = MicrovixConverters.ToNullableDecimal(dto.dist_visao);
            personalizada = MicrovixConverters.ToNullableString(dto.personalizada, 100);
            pris_od = MicrovixConverters.ToNullableDecimal(dto.pris_od);
            pris_oe = MicrovixConverters.ToNullableDecimal(dto.pris_oe);
            eixo_od = MicrovixConverters.ToNullableDecimal(dto.eixo_od);
            eixo_oe = MicrovixConverters.ToNullableDecimal(dto.eixo_oe);
            borda_od = MicrovixConverters.ToNullableDecimal(dto.borda_od);
            borda_oe = MicrovixConverters.ToNullableDecimal(dto.borda_oe);
            centro_od = MicrovixConverters.ToNullableDecimal(dto.centro_od);
            centro_oe = MicrovixConverters.ToNullableDecimal(dto.centro_oe);
            furo_od = MicrovixConverters.ToNullableDecimal(dto.furo_od);
            furo_oe = MicrovixConverters.ToNullableDecimal(dto.furo_oe);
            format_corte = MicrovixConverters.ToNullableString(dto.format_corte, 100);
            dist_leitura = MicrovixConverters.ToNullableString(dto.dist_leitura, 100);
            dados_armacao_informados_manualmente = MicrovixConverters.ToNullableBoolean(dto.dados_armacao_informados_manualmente);
            codigo_gerencial_2 = MicrovixConverters.ToNullableString(dto.codigo_gerencial_2, 1000);   //fora da documentação
            pronta_entrega = MicrovixConverters.ToNullableBoolean(dto.pronta_entrega);
            cod_cliente_medico_especialista = MicrovixConverters.ToNullableInt32(dto.cod_cliente_medico_especialista);
            cod_cliente_clinica = MicrovixConverters.ToNullableInt32(dto.cod_cliente_clinica);
            vendedor = MicrovixConverters.ToNullableInt32(dto.vendedor);
        }
    }
}
