using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings
{
    /// <summary>
    /// LinxConfiguracoesTributariasDetalhesSimplificado, tabela linxmicrovix.LinxConfiguracoesTributariasDetalhesSimplificado. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_config_tributaria_detalhe
    /// </summary>
    public class LinxConfiguracoesTributariasDetalhesSimplificado
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_config_tributaria_detalhe { get; private set; }
        public int? id_config_tributaria { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public int? id_classe_fiscal { get; private set; }
        public int? id_cst_icms_fiscal { get; private set; }
        public int? id_csosn_fiscal { get; private set; }
        public int? id_cfop_fiscal { get; private set; }
        public bool? ipi_credito { get; private set; }
        public bool? icms_credito { get; private set; }
        public decimal? aliq_icms { get; private set; }
        public decimal? perc_reducao_icms { get; private set; }
        public decimal? aliquota_st { get; private set; }
        public decimal? margem_st { get; private set; }
        public long timestamp { get; private set; }

        public LinxConfiguracoesTributariasDetalhesSimplificado() { }

        public LinxConfiguracoesTributariasDetalhesSimplificado(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_config_tributaria_detalhe = MicrovixConverters.ToInt32(dto.id_config_tributaria_detalhe);
            id_config_tributaria = MicrovixConverters.ToNullableInt32(dto.id_config_tributaria);
            cod_natureza_operacao = MicrovixConverters.ToNullableString(dto.cod_natureza_operacao, 10);
            id_classe_fiscal = MicrovixConverters.ToNullableInt32(dto.id_classe_fiscal);
            id_cst_icms_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cst_icms_fiscal);
            id_csosn_fiscal = MicrovixConverters.ToNullableInt32(dto.id_csosn_fiscal);
            id_cfop_fiscal = MicrovixConverters.ToNullableInt32(dto.id_cfop_fiscal);
            ipi_credito = MicrovixConverters.ToNullableBoolean(dto.ipi_credito);
            icms_credito = MicrovixConverters.ToNullableBoolean(dto.icms_credito);
            aliq_icms = MicrovixConverters.ToNullableDecimal(dto.aliq_icms);
            perc_reducao_icms = MicrovixConverters.ToNullableDecimal(dto.perc_reducao_icms);
            aliquota_st = MicrovixConverters.ToNullableDecimal(dto.aliquota_st);
            margem_st = MicrovixConverters.ToNullableDecimal(dto.margem_st);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
