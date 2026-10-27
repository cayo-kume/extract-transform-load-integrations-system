using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings
{
    /// <summary>
    /// LinxConfiguracoesTributarias, tabela linxmicrovix.LinxConfiguracoesTributarias. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_config_tributaria (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxConfiguracoesTributarias
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_config_tributaria { get; private set; }
        public string? desc_config_tributaria { get; private set; }
        public string? sigla_config_tributaria { get; private set; }
        public long timestamp { get; private set; }
        public bool? ativa { get; private set; }
        public string? uf { get; private set; }
        public string? sistema_tributacao { get; private set; }
        public int? tipo_atividade { get; private set; }
        public int? id_origem_mercadoria { get; private set; }
        public bool? utiliza_uso_consumo { get; private set; }
        public int? id_classificacao_cest_produto { get; private set; }
        public string? codigo_ws { get; private set; }

        public LinxConfiguracoesTributarias() { }

        public LinxConfiguracoesTributarias(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_config_tributaria = MicrovixConverters.ToInt32(dto.id_config_tributaria);
            desc_config_tributaria = MicrovixConverters.ToNullableString(dto.desc_config_tributaria, 100);
            sigla_config_tributaria = MicrovixConverters.ToNullableString(dto.sigla_config_tributaria, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            ativa = MicrovixConverters.ToNullableBoolean(dto.ativa);
            uf = MicrovixConverters.ToNullableString(dto.uf, 2);
            sistema_tributacao = MicrovixConverters.ToNullableString(dto.sistema_tributacao, 1);
            tipo_atividade = MicrovixConverters.ToNullableInt32(dto.tipo_atividade);
            id_origem_mercadoria = MicrovixConverters.ToNullableInt32(dto.id_origem_mercadoria);
            utiliza_uso_consumo = MicrovixConverters.ToNullableBoolean(dto.utiliza_uso_consumo);
            id_classificacao_cest_produto = MicrovixConverters.ToNullableInt32(dto.id_classificacao_cest_produto);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 50);
        }
    }
}
