using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxConhecimentoFretes, tabela linxmicrovix.LinxConhecimentoFretes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_conhecimento (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxConhecimentoFretes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_conhecimento { get; private set; }
        public int? transportador { get; private set; }
        public decimal? valor_frete { get; private set; }
        public int? conhecimento { get; private set; }
        public string? serie_frete { get; private set; }
        public string? tipo_frete { get; private set; }
        public DateTime? vencto_frete { get; private set; }
        public decimal? valor_total_notas { get; private set; }
        public decimal? valor_lancado { get; private set; }
        public DateTime? data_emissao { get; private set; }
        public DateTime? data_entrada { get; private set; }
        public string? cfop { get; private set; }
        public decimal? aliq_icms { get; private set; }
        public int? empresa { get; private set; }
        public decimal? base_calculo_icms { get; private set; }
        public int? id_cfop_fiscal { get; private set; }
        public int? id_modelo_nf { get; private set; }
        public int? id_cte_tipo { get; private set; }
        public decimal? valor_icms { get; private set; }
        public decimal? valor_pis { get; private set; }
        public decimal? valor_cofins { get; private set; }
        public string? chave_cte { get; private set; }
        public string? regime_pis_cofins { get; private set; }
        public decimal? base_pis { get; private set; }
        public decimal? base_cofins { get; private set; }
        public int? id_cst_icms_fiscal { get; private set; }
        public int? id_nfe_ibge_municipio_origem { get; private set; }
        public int? id_nfe_ibge_municipio_destino { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxConhecimentoFretes() { }

        public LinxConhecimentoFretes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxConhecimentoFretes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_conhecimento = MicrovixConverters.ToInt32(dto.id_conhecimento);
            transportador = MicrovixConverters.ToNullableInt32(dto.transportador);
            valor_frete = MicrovixConverters.ToNullableDecimal(dto.valor_frete);
            conhecimento = MicrovixConverters.ToNullableInt32(dto.conhecimento);
            serie_frete = MicrovixConverters.ToNullableString(dto.serie_frete, 10);
            tipo_frete = String.IsNullOrEmpty(dto.tipo_frete) ? null : dto.tipo_frete;
            vencto_frete = MicrovixConverters.ToNullableDateTime(dto.vencto_frete);
            valor_total_notas = MicrovixConverters.ToNullableDecimal(dto.valor_total_notas);
            valor_lancado = MicrovixConverters.ToNullableDecimal(dto.valor_lancado);
            data_emissao = MicrovixConverters.ToNullableDateTime(dto.data_emissao);
            data_entrada = MicrovixConverters.ToNullableDateTime(dto.data_entrada);
            cfop = String.IsNullOrEmpty(dto.cfop) ? null : dto.cfop;
            aliq_icms = MicrovixConverters.ToNullableDecimal(dto.aliq_icms);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            base_calculo_icms = MicrovixConverters.ToNullableDecimal(dto.base_calculo_icms);
            id_cfop_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cfop_fiscal);
            id_modelo_nf = MicrovixConverters.ToNullableInt32(dto.id_modelo_nf);
            id_cte_tipo = MicrovixConverters.ToNullableInt32(dto.id_cte_tipo);
            valor_icms = MicrovixConverters.ToNullableDecimal(dto.valor_icms);
            valor_pis = MicrovixConverters.ToNullableDecimal(dto.valor_pis);
            valor_cofins = MicrovixConverters.ToNullableDecimal(dto.valor_cofins);
            chave_cte = MicrovixConverters.ToNullableString(dto.chave_cte, 44);
            regime_pis_cofins = String.IsNullOrEmpty(dto.regime_pis_cofins) ? null : dto.regime_pis_cofins;
            base_pis = MicrovixConverters.ToNullableDecimal(dto.base_pis);
            base_cofins = MicrovixConverters.ToNullableDecimal(dto.base_cofins);
            id_cst_icms_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_icms_fiscal);
            id_nfe_ibge_municipio_origem = MicrovixConverters.ToNullableInt32(dto.id_nfe_ibge_municipio_origem);
            id_nfe_ibge_municipio_destino = MicrovixConverters.ToNullableInt32(dto.id_nfe_ibge_municipio_destino);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
