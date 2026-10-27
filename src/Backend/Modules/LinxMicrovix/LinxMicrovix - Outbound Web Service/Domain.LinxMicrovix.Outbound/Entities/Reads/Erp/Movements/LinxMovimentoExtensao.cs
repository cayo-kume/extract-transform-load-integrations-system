using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoExtensao, tabela linxmicrovix.LinxMovimentoExtensao. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao
    /// </summary>
    public class LinxMovimentoExtensao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int transacao { get; private set; }
        public Guid? identificador { get; private set; }
        public decimal? base_fcp_st { get; private set; }
        public decimal? valor_fcp_st { get; private set; }
        public decimal? aliq_fcp_st { get; private set; }
        public decimal? base_icms_fcp_st { get; private set; }
        public decimal? valor_icms_fcp_st { get; private set; }
        public decimal? base_icms_fcp_st_retido { get; private set; }
        public decimal? valor_icms_fcp_st_retido { get; private set; }
        public decimal? base_icms_fcp_st_antecipado { get; private set; }
        public decimal? valor_icms_fcp_st_antecipado { get; private set; }
        public decimal? aliquota_icms_fcp_st_antecipado { get; private set; }
        public long timestamp { get; private set; }
        public decimal? valor_iss { get; private set; }
        public int? tipo_tributacao_iss { get; private set; }
        public decimal? icms_fcp_aliquota { get; private set; }
        public decimal? icms_fcp_base_item { get; private set; }
        public decimal? icms_fcp_valor_item { get; private set; }
        public decimal? icms_base_partilha { get; private set; }
        public decimal? aliq_difal_interna_uf_destinatario { get; private set; }
        public decimal? aliq_difal_interestadual_uf_envolvidas { get; private set; }
        public decimal? icms_item_perc_partilha_destino { get; private set; }
        public decimal? icms_item_perc_partilha_origem { get; private set; }
        public long? codigo_pacote { get; private set; }
        public string? codigo_itens_associados { get; private set; }
        public long? codigo_kit { get; private set; }
        public int? id_motivo_desconto { get; private set; }
        public decimal? icms_st_antecipado_base_item { get; private set; }
        public decimal? icms_suportado_valor_item { get; private set; }
        public decimal? icms_suportado_valor_unitario { get; private set; }
        public decimal? icms_st_pago_base { get; private set; }
        public decimal? icms_st_pago_valor { get; private set; }
        public decimal? icms_st_pago_aliq { get; private set; }
        public decimal? icms_para_st_pago_valor { get; private set; }
        public int? cod_cliente_indicador { get; private set; }
        public string? motivo_desconto_observacao { get; private set; }
        public string? codigo_classificacao_tributaria { get; private set; }
        public string? codigo_cst_cbs_ibs { get; private set; }
        public decimal? base_cbs_ibs_item { get; private set; }
        public decimal? aliquota_cbs { get; private set; }
        public decimal? valor_cbs_item { get; private set; }
        public decimal? percentual_reducao_cbs { get; private set; }
        public decimal? aliquota_efetiva_cbs { get; private set; }
        public decimal? aliquota_ibs_uf { get; private set; }
        public decimal? valor_ibs_uf_item { get; private set; }
        public decimal? percentual_reducao_ibs_uf { get; private set; }
        public decimal? aliquota_efetiva_ibs_uf { get; private set; }
        public decimal? aliquota_ibs_municipio { get; private set; }
        public decimal? valor_ibs_municipio_item { get; private set; }
        public decimal? percentual_reducao_ibs_municipio { get; private set; }
        public decimal? aliquota_efetiva_ibs_municipio { get; private set; }
        public decimal? valor_ibs_total_item { get; private set; }
        public decimal? custo_aquisicao_item { get; private set; }

        public LinxMovimentoExtensao() { }

        public LinxMovimentoExtensao(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoExtensao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            base_fcp_st = MicrovixConverters.ToNullableDecimal(dto.base_fcp_st);
            valor_fcp_st = MicrovixConverters.ToNullableDecimal(dto.valor_fcp_st);
            aliq_fcp_st = MicrovixConverters.ToNullableDecimal(dto.aliq_fcp_st);
            base_icms_fcp_st = MicrovixConverters.ToNullableDecimal(dto.base_icms_fcp_st);
            valor_icms_fcp_st = MicrovixConverters.ToNullableDecimal(dto.valor_icms_fcp_st);
            base_icms_fcp_st_retido = MicrovixConverters.ToNullableDecimal(dto.base_icms_fcp_st_retido);
            valor_icms_fcp_st_retido = MicrovixConverters.ToNullableDecimal(dto.valor_icms_fcp_st_retido);
            base_icms_fcp_st_antecipado = MicrovixConverters.ToNullableDecimal(dto.base_icms_fcp_st_antecipado);
            valor_icms_fcp_st_antecipado = MicrovixConverters.ToNullableDecimal(dto.valor_icms_fcp_st_antecipado);
            aliquota_icms_fcp_st_antecipado = MicrovixConverters.ToNullableDecimal(dto.aliquota_icms_fcp_st_antecipado);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            valor_iss = MicrovixConverters.ToNullableDecimal(dto.valor_iss);
            tipo_tributacao_iss = MicrovixConverters.ToNullableInt32(dto.tipo_tributacao_iss);
            icms_fcp_aliquota = MicrovixConverters.ToNullableDecimal(dto.icms_fcp_aliquota);
            icms_fcp_base_item = MicrovixConverters.ToNullableDecimal(dto.icms_fcp_base_item);
            icms_fcp_valor_item = MicrovixConverters.ToNullableDecimal(dto.icms_fcp_valor_item);
            icms_base_partilha = MicrovixConverters.ToNullableDecimal(dto.icms_base_partilha);
            aliq_difal_interna_uf_destinatario = MicrovixConverters.ToNullableDecimal(dto.aliq_difal_interna_uf_destinatario);
            aliq_difal_interestadual_uf_envolvidas = MicrovixConverters.ToNullableDecimal(dto.aliq_difal_interestadual_uf_envolvidas);
            icms_item_perc_partilha_destino = MicrovixConverters.ToNullableDecimal(dto.icms_item_perc_partilha_destino);
            icms_item_perc_partilha_origem = MicrovixConverters.ToNullableDecimal(dto.icms_item_perc_partilha_origem);
            codigo_pacote = MicrovixConverters.ToNullableInt64(dto.codigo_pacote);
            codigo_itens_associados = MicrovixConverters.ToNullableString(dto.codigo_itens_associados, 1000);   //fora da documentação
            codigo_kit = MicrovixConverters.ToNullableInt64(dto.codigo_kit);
            id_motivo_desconto = MicrovixConverters.ToNullableInt32(dto.id_motivo_desconto);
            icms_st_antecipado_base_item = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_base_item);
            icms_suportado_valor_item = MicrovixConverters.ToNullableDecimal(dto.icms_suportado_valor_item);
            icms_suportado_valor_unitario = MicrovixConverters.ToNullableDecimal(dto.icms_suportado_valor_unitario);
            icms_st_pago_base = MicrovixConverters.ToNullableDecimal(dto.icms_st_pago_base);
            icms_st_pago_valor = MicrovixConverters.ToNullableDecimal(dto.icms_st_pago_valor);
            icms_st_pago_aliq = MicrovixConverters.ToNullableDecimal(dto.icms_st_pago_aliq);
            icms_para_st_pago_valor = MicrovixConverters.ToNullableDecimal(dto.icms_para_st_pago_valor);
            cod_cliente_indicador = MicrovixConverters.ToNullableInt32(dto.cod_cliente_indicador);
            motivo_desconto_observacao = MicrovixConverters.ToNullableString(dto.motivo_desconto_observacao, 500);
            codigo_classificacao_tributaria = MicrovixConverters.ToNullableString(dto.codigo_classificacao_tributaria, 6);
            codigo_cst_cbs_ibs = MicrovixConverters.ToNullableString(dto.codigo_cst_cbs_ibs, 3);
            base_cbs_ibs_item = MicrovixConverters.ToNullableDecimal(dto.base_cbs_ibs_item);
            aliquota_cbs = MicrovixConverters.ToNullableDecimal(dto.aliquota_cbs);
            valor_cbs_item = MicrovixConverters.ToNullableDecimal(dto.valor_cbs_item);
            percentual_reducao_cbs = MicrovixConverters.ToNullableDecimal(dto.percentual_reducao_cbs);
            aliquota_efetiva_cbs = MicrovixConverters.ToNullableDecimal(dto.aliquota_efetiva_cbs);
            aliquota_ibs_uf = MicrovixConverters.ToNullableDecimal(dto.aliquota_ibs_uf);
            valor_ibs_uf_item = MicrovixConverters.ToNullableDecimal(dto.valor_ibs_uf_item);
            percentual_reducao_ibs_uf = MicrovixConverters.ToNullableDecimal(dto.percentual_reducao_ibs_uf);
            aliquota_efetiva_ibs_uf = MicrovixConverters.ToNullableDecimal(dto.aliquota_efetiva_ibs_uf);
            aliquota_ibs_municipio = MicrovixConverters.ToNullableDecimal(dto.aliquota_ibs_municipio);
            valor_ibs_municipio_item = MicrovixConverters.ToNullableDecimal(dto.valor_ibs_municipio_item);
            percentual_reducao_ibs_municipio = MicrovixConverters.ToNullableDecimal(dto.percentual_reducao_ibs_municipio);
            aliquota_efetiva_ibs_municipio = MicrovixConverters.ToNullableDecimal(dto.aliquota_efetiva_ibs_municipio);
            valor_ibs_total_item = MicrovixConverters.ToNullableDecimal(dto.valor_ibs_total_item);
            custo_aquisicao_item = MicrovixConverters.ToNullableDecimal(dto.custo_aquisicao_item);
        }
    }
}
