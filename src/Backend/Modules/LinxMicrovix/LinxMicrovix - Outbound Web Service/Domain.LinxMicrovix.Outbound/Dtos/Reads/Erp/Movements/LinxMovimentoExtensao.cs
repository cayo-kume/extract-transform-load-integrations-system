namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoExtensao: as informações adicionais da venda (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoExtensao
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? transacao { get; init; }
        public string? identificador { get; init; }
        public string? base_fcp_st { get; init; }
        public string? valor_fcp_st { get; init; }
        public string? aliq_fcp_st { get; init; }
        public string? base_icms_fcp_st { get; init; }
        public string? valor_icms_fcp_st { get; init; }
        public string? base_icms_fcp_st_retido { get; init; }
        public string? valor_icms_fcp_st_retido { get; init; }
        public string? base_icms_fcp_st_antecipado { get; init; }
        public string? valor_icms_fcp_st_antecipado { get; init; }
        public string? aliquota_icms_fcp_st_antecipado { get; init; }
        public string? timestamp { get; init; }
        public string? valor_iss { get; init; }
        public string? tipo_tributacao_iss { get; init; }
        public string? icms_fcp_aliquota { get; init; }
        public string? icms_fcp_base_item { get; init; }
        public string? icms_fcp_valor_item { get; init; }
        public string? icms_base_partilha { get; init; }
        public string? aliq_difal_interna_uf_destinatario { get; init; }
        public string? aliq_difal_interestadual_uf_envolvidas { get; init; }
        public string? icms_item_perc_partilha_destino { get; init; }
        public string? icms_item_perc_partilha_origem { get; init; }
        public string? codigo_pacote { get; init; }
        public string? codigo_itens_associados { get; init; }
        public string? codigo_kit { get; init; }
        public string? id_motivo_desconto { get; init; }
        public string? icms_st_antecipado_base_item { get; init; }
        public string? icms_suportado_valor_item { get; init; }
        public string? icms_suportado_valor_unitario { get; init; }
        public string? icms_st_pago_base { get; init; }
        public string? icms_st_pago_valor { get; init; }
        public string? icms_st_pago_aliq { get; init; }
        public string? icms_para_st_pago_valor { get; init; }
        public string? cod_cliente_indicador { get; init; }
        public string? motivo_desconto_observacao { get; init; }
        public string? codigo_classificacao_tributaria { get; init; }
        public string? codigo_cst_cbs_ibs { get; init; }
        public string? base_cbs_ibs_item { get; init; }
        public string? aliquota_cbs { get; init; }
        public string? valor_cbs_item { get; init; }
        public string? percentual_reducao_cbs { get; init; }
        public string? aliquota_efetiva_cbs { get; init; }
        public string? aliquota_ibs_uf { get; init; }
        public string? valor_ibs_uf_item { get; init; }
        public string? percentual_reducao_ibs_uf { get; init; }
        public string? aliquota_efetiva_ibs_uf { get; init; }
        public string? aliquota_ibs_municipio { get; init; }
        public string? valor_ibs_municipio_item { get; init; }
        public string? percentual_reducao_ibs_municipio { get; init; }
        public string? aliquota_efetiva_ibs_municipio { get; init; }
        public string? valor_ibs_total_item { get; init; }
        public string? custo_aquisicao_item { get; init; }
    }
}
