using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings
{
    /// <summary>
    /// LinxConfiguracoesTributariasDetalhes, tabela linxmicrovix.LinxConfiguracoesTributariasDetalhes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_config_tributaria + cod_natureza_operacao + id_classe_fiscal (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxConfiguracoesTributariasDetalhes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_config_tributaria { get; private set; }
        public string? desc_classe_fiscal { get; private set; }
        public string cod_natureza_operacao { get; private set; } = String.Empty;
        public string? desc_natureza_operacao { get; private set; }
        public string? cfop_fiscal { get; private set; }
        public string? desc_cfop_fiscal { get; private set; }
        public decimal? aliq_icms { get; private set; }
        public decimal? valor_tributado_icms { get; private set; }
        public decimal? aliq_pis { get; private set; }
        public decimal? aliq_cofins { get; private set; }
        public decimal? perc_reducao_icms { get; private set; }
        public decimal? perc_reducao_icms_st { get; private set; }
        public decimal? margem_st { get; private set; }
        public decimal? aliquota_st { get; private set; }
        public decimal? margem_st_simulador { get; private set; }
        public decimal? aliquota_st_simulador { get; private set; }
        public decimal? desconto_icms { get; private set; }
        public string? cst_icms_fiscal { get; private set; }
        public string? desc_cst_icms_fiscal { get; private set; }
        public string? cst_ipi_fiscal { get; private set; }
        public string? desc_cst_ipi_fiscal { get; private set; }
        public string? cst_pis_fiscal { get; private set; }
        public string? desc_cst_pis_fiscal { get; private set; }
        public int? cst_cofins_fiscal { get; private set; }
        public string? desc_cst_cofins_fiscal { get; private set; }
        public string? desc_obs_padrao { get; private set; }
        public bool? icms_credito { get; private set; }
        public bool? ipi_credito { get; private set; }
        public string? cod_enquadramento_ipi { get; private set; }
        public string? desc_enquadramento_ipi { get; private set; }
        public decimal? perc_aliquota_interna_uf_destinatario { get; private set; }
        public decimal? perc_aliquota_interestadual_uf_envolvidas { get; private set; }
        public string? csosn_fiscal { get; private set; }
        public string? desc_csosn_fiscal { get; private set; }
        public string? forma_tributacao_pis { get; private set; }
        public string? forma_tributacao_cofins { get; private set; }
        public int? id_ramo_atividade { get; private set; }
        public int? empresa { get; private set; }
        public bool? usa_base_icms_para_calculo_st { get; private set; }
        public int id_classe_fiscal { get; private set; }
        public int? id_cfop_fiscal { get; private set; }
        public int? id_cst_icms_fiscal { get; private set; }
        public int? id_cst_ipi_fiscal { get; private set; }
        public int? id_cst_pis_fiscal { get; private set; }
        public int? id_cst_cofins_fiscal { get; private set; }
        public int? id_obs { get; private set; }
        public bool? tributado { get; private set; }
        public bool? retido { get; private set; }
        public int? icms_base_naotributado { get; private set; }
        public int? ipi_base_naotributado { get; private set; }
        public int? id_csosn_fiscal { get; private set; }
        public bool? somar_icms_st { get; private set; }
        public int? id_sped_tipo_base_credito { get; private set; }
        public decimal? aliq_pis_servico { get; private set; }
        public decimal? aliq_cofins_servico { get; private set; }
        public int? id_cst_pis_fiscal_servico { get; private set; }
        public int? id_cst_cofins_fiscal_servico { get; private set; }
        public bool? csrf { get; private set; }
        public string? codigo_retencao_csrf { get; private set; }
        public decimal? aliq_pis_csrf { get; private set; }
        public decimal? aliq_cofins_csrf { get; private set; }
        public decimal? aliq_csll_csrf { get; private set; }
        public int? id_sped_tipo_base_credito_servico { get; private set; }
        public bool? receita { get; private set; }
        public bool? fci_informa_parcela_importada { get; private set; }
        public bool? fci_informa_numero { get; private set; }
        public bool? fci_informa_conteudo_importacao { get; private set; }
        public bool? fci_informa_valor_importacao { get; private set; }
        public int? iss_tipo_tributacao { get; private set; }
        public decimal? icms_st_antecipado_margem { get; private set; }
        public decimal? icms_st_antecipado_aliquota { get; private set; }
        public decimal? icms_st_antecipado_percentual_reducao { get; private set; }
        public bool? icms_st_antecipado_valor_integra_custo_medio { get; private set; }
        public int? id_enquadramento_ipi { get; private set; }
        public bool? usa_regime_estimativa_simplifica { get; private set; }
        public decimal? aliq_carga_tributaria_media { get; private set; }
        public bool? fcp_integra_custo_medio { get; private set; }
        public decimal? aliq_fcp { get; private set; }
        public bool? fcp_credito { get; private set; }
        public bool? fcp_reducao_icms { get; private set; }
        public decimal? aliq_icms_efetivo { get; private set; }
        public decimal? perc_reducao_icms_efetivo { get; private set; }
        public decimal? icms_fcp_st_antecipado_aliquota { get; private set; }
        public bool? difal_base_dupla { get; private set; }
        public decimal? aliq_desoneracao_icms { get; private set; }
        public decimal? aliq_desoneracao_fcp { get; private set; }
        public string? cod_beneficio_fiscal { get; private set; }
        public int? id_motivo_desoneracao_icms { get; private set; }
        public bool? deduzir_icms_custo { get; private set; }
        public bool? deduzir_icms_desonerado { get; private set; }
        public bool? somar_ipi_base_difal_fcp { get; private set; }
        public int? tipo_base_dupla { get; private set; }
        public bool? utiliza_lucro_base_icms { get; private set; }
        public bool? utiliza_lucro_base_pis { get; private set; }
        public bool? utiliza_lucro_base_cofins { get; private set; }
        public string? codigo_ws { get; private set; }
        public long timestamp { get; private set; }
        public string? aliquota_diferimento_icms { get; private set; }
        public decimal? aliquota_diferimento_fcp { get; private set; }

        public LinxConfiguracoesTributariasDetalhes() { }

        public LinxConfiguracoesTributariasDetalhes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_config_tributaria = MicrovixConverters.ToInt32(dto.id_config_tributaria);
            desc_classe_fiscal = MicrovixConverters.ToNullableString(dto.desc_classe_fiscal, 150);
            cod_natureza_operacao = MicrovixConverters.ToNullableString(dto.cod_natureza_operacao, 10) ?? String.Empty;
            desc_natureza_operacao = MicrovixConverters.ToNullableString(dto.desc_natureza_operacao, 60);
            cfop_fiscal = MicrovixConverters.ToNullableString(dto.cfop_fiscal, 5);
            desc_cfop_fiscal = MicrovixConverters.ToNullableString(dto.desc_cfop_fiscal, 300);
            aliq_icms = MicrovixConverters.ToNullableDecimal(dto.aliq_icms);
            valor_tributado_icms = MicrovixConverters.ToNullableDecimal(dto.valor_tributado_icms);
            aliq_pis = MicrovixConverters.ToNullableDecimal(dto.aliq_pis);
            aliq_cofins = MicrovixConverters.ToNullableDecimal(dto.aliq_cofins);
            perc_reducao_icms = MicrovixConverters.ToNullableDecimal(dto.perc_reducao_icms);
            perc_reducao_icms_st = MicrovixConverters.ToNullableDecimal(dto.perc_reducao_icms_st);
            margem_st = MicrovixConverters.ToNullableDecimal(dto.margem_st);
            aliquota_st = MicrovixConverters.ToNullableDecimal(dto.aliquota_st);
            margem_st_simulador = MicrovixConverters.ToNullableDecimal(dto.margem_st_simulador);
            aliquota_st_simulador = MicrovixConverters.ToNullableDecimal(dto.aliquota_st_simulador);
            desconto_icms = MicrovixConverters.ToNullableDecimal(dto.desconto_icms);
            cst_icms_fiscal = MicrovixConverters.ToNullableString(dto.cst_icms_fiscal, 4);
            desc_cst_icms_fiscal = MicrovixConverters.ToNullableString(dto.desc_cst_icms_fiscal, 150);
            cst_ipi_fiscal = MicrovixConverters.ToNullableString(dto.cst_ipi_fiscal, 4);
            desc_cst_ipi_fiscal = MicrovixConverters.ToNullableString(dto.desc_cst_ipi_fiscal, 150);
            cst_pis_fiscal = MicrovixConverters.ToNullableString(dto.cst_pis_fiscal, 4);
            desc_cst_pis_fiscal = MicrovixConverters.ToNullableString(dto.desc_cst_pis_fiscal, 150);
            cst_cofins_fiscal = MicrovixConverters.ToNullableInt32(dto.cst_cofins_fiscal);   //fora da documentação
            desc_cst_cofins_fiscal = MicrovixConverters.ToNullableString(dto.desc_cst_cofins_fiscal, 100);   //fora da documentação
            desc_obs_padrao = MicrovixConverters.ToNullableString(dto.desc_obs_padrao, 250);
            icms_credito = MicrovixConverters.ToNullableBoolean(dto.icms_credito);
            ipi_credito = MicrovixConverters.ToNullableBoolean(dto.ipi_credito);   //fora da documentação
            cod_enquadramento_ipi = MicrovixConverters.ToNullableString(dto.cod_enquadramento_ipi, 3);
            desc_enquadramento_ipi = MicrovixConverters.ToNullableString(dto.desc_enquadramento_ipi, 600);
            perc_aliquota_interna_uf_destinatario = MicrovixConverters.ToNullableDecimal(dto.perc_aliquota_interna_uf_destinatario);
            perc_aliquota_interestadual_uf_envolvidas = MicrovixConverters.ToNullableDecimal(dto.perc_aliquota_interestadual_uf_envolvidas);
            csosn_fiscal = MicrovixConverters.ToNullableString(dto.csosn_fiscal, 5);
            desc_csosn_fiscal = MicrovixConverters.ToNullableString(dto.desc_csosn_fiscal, 200);
            forma_tributacao_pis = MicrovixConverters.ToNullableString(dto.forma_tributacao_pis, 10);
            forma_tributacao_cofins = MicrovixConverters.ToNullableString(dto.forma_tributacao_cofins, 10);
            id_ramo_atividade = MicrovixConverters.ToNullableInt32(dto.id_ramo_atividade);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            usa_base_icms_para_calculo_st = MicrovixConverters.ToNullableBoolean(dto.usa_base_icms_para_calculo_st);
            id_classe_fiscal = MicrovixConverters.ToInt32(dto.id_classe_fiscal);
            id_cfop_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cfop_fiscal);
            id_cst_icms_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_icms_fiscal);
            id_cst_ipi_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_ipi_fiscal);
            id_cst_pis_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_pis_fiscal);   //fora da documentação
            id_cst_cofins_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_cofins_fiscal);   //fora da documentação
            id_obs = MicrovixConverters.ToNullableInt32(dto.id_obs);
            tributado = MicrovixConverters.ToNullableBoolean(dto.tributado);
            retido = MicrovixConverters.ToNullableBoolean(dto.retido);
            icms_base_naotributado = MicrovixConverters.ToNullableInt32(dto.icms_base_naotributado);
            ipi_base_naotributado = MicrovixConverters.ToNullableInt32(dto.ipi_base_naotributado);
            id_csosn_fiscal = MicrovixConverters.ToNullableInt32(dto.id_csosn_fiscal);
            somar_icms_st = MicrovixConverters.ToNullableBoolean(dto.somar_icms_st);
            id_sped_tipo_base_credito = MicrovixConverters.ToNullableInt32(dto.id_sped_tipo_base_credito);
            aliq_pis_servico = MicrovixConverters.ToNullableDecimal(dto.aliq_pis_servico);   //fora da documentação
            aliq_cofins_servico = MicrovixConverters.ToNullableDecimal(dto.aliq_cofins_servico);
            id_cst_pis_fiscal_servico = MicrovixConverters.ToNullableInt32(dto.id_cst_pis_fiscal_servico);
            id_cst_cofins_fiscal_servico = MicrovixConverters.ToNullableInt32(dto.id_cst_cofins_fiscal_servico);
            csrf = MicrovixConverters.ToNullableBoolean(dto.csrf);
            codigo_retencao_csrf = MicrovixConverters.ToNullableString(dto.codigo_retencao_csrf, 4);
            aliq_pis_csrf = MicrovixConverters.ToNullableDecimal(dto.aliq_pis_csrf);
            aliq_cofins_csrf = MicrovixConverters.ToNullableDecimal(dto.aliq_cofins_csrf);
            aliq_csll_csrf = MicrovixConverters.ToNullableDecimal(dto.aliq_csll_csrf);
            id_sped_tipo_base_credito_servico = MicrovixConverters.ToNullableInt32(dto.id_sped_tipo_base_credito_servico);
            receita = MicrovixConverters.ToNullableBoolean(dto.receita);
            fci_informa_parcela_importada = MicrovixConverters.ToNullableBoolean(dto.fci_informa_parcela_importada);
            fci_informa_numero = MicrovixConverters.ToNullableBoolean(dto.fci_informa_numero);
            fci_informa_conteudo_importacao = MicrovixConverters.ToNullableBoolean(dto.fci_informa_conteudo_importacao);
            fci_informa_valor_importacao = MicrovixConverters.ToNullableBoolean(dto.fci_informa_valor_importacao);
            iss_tipo_tributacao = MicrovixConverters.ToNullableInt32(dto.iss_tipo_tributacao);
            icms_st_antecipado_margem = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_margem);
            icms_st_antecipado_aliquota = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_aliquota);
            icms_st_antecipado_percentual_reducao = MicrovixConverters.ToNullableDecimal(dto.icms_st_antecipado_percentual_reducao);
            icms_st_antecipado_valor_integra_custo_medio = MicrovixConverters.ToNullableBoolean(dto.icms_st_antecipado_valor_integra_custo_medio);
            id_enquadramento_ipi = MicrovixConverters.ToNullableInt32(dto.id_enquadramento_ipi);
            usa_regime_estimativa_simplifica = MicrovixConverters.ToNullableBoolean(dto.usa_regime_estimativa_simplifica);
            aliq_carga_tributaria_media = MicrovixConverters.ToNullableDecimal(dto.aliq_carga_tributaria_media);
            fcp_integra_custo_medio = MicrovixConverters.ToNullableBoolean(dto.fcp_integra_custo_medio);
            aliq_fcp = MicrovixConverters.ToNullableDecimal(dto.aliq_fcp);
            fcp_credito = MicrovixConverters.ToNullableBoolean(dto.fcp_credito);
            fcp_reducao_icms = MicrovixConverters.ToNullableBoolean(dto.fcp_reducao_icms);
            aliq_icms_efetivo = MicrovixConverters.ToNullableDecimal(dto.aliq_icms_efetivo);
            perc_reducao_icms_efetivo = MicrovixConverters.ToNullableDecimal(dto.perc_reducao_icms_efetivo);
            icms_fcp_st_antecipado_aliquota = MicrovixConverters.ToNullableDecimal(dto.icms_fcp_st_antecipado_aliquota);
            difal_base_dupla = MicrovixConverters.ToNullableBoolean(dto.difal_base_dupla);
            aliq_desoneracao_icms = MicrovixConverters.ToNullableDecimal(dto.aliq_desoneracao_icms);
            aliq_desoneracao_fcp = MicrovixConverters.ToNullableDecimal(dto.aliq_desoneracao_fcp);
            cod_beneficio_fiscal = MicrovixConverters.ToNullableString(dto.cod_beneficio_fiscal, 10);
            id_motivo_desoneracao_icms = MicrovixConverters.ToNullableInt32(dto.id_motivo_desoneracao_icms);
            deduzir_icms_custo = MicrovixConverters.ToNullableBoolean(dto.deduzir_icms_custo);
            deduzir_icms_desonerado = MicrovixConverters.ToNullableBoolean(dto.deduzir_icms_desonerado);
            somar_ipi_base_difal_fcp = MicrovixConverters.ToNullableBoolean(dto.somar_ipi_base_difal_fcp);
            tipo_base_dupla = MicrovixConverters.ToNullableInt32(dto.tipo_base_dupla);
            utiliza_lucro_base_icms = MicrovixConverters.ToNullableBoolean(dto.utiliza_lucro_base_icms);
            utiliza_lucro_base_pis = MicrovixConverters.ToNullableBoolean(dto.utiliza_lucro_base_pis);
            utiliza_lucro_base_cofins = MicrovixConverters.ToNullableBoolean(dto.utiliza_lucro_base_cofins);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            aliquota_diferimento_icms = MicrovixConverters.ToNullableString(dto.aliquota_diferimento_icms, 1000);   //fora da documentação
            aliquota_diferimento_fcp = MicrovixConverters.ToNullableDecimal(dto.aliquota_diferimento_fcp);
        }
    }
}
