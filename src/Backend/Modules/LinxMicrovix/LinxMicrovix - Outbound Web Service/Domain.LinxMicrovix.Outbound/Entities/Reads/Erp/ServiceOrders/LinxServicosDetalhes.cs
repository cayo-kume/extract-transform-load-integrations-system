using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxServicosDetalhes, tabela linxmicrovix.LinxServicosDetalhes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_servico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxServicosDetalhes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_servico { get; private set; }
        public decimal? preco_custo_hn { get; private set; }
        public decimal? preco_primera_hn { get; private set; }
        public decimal? preco_demais_hn { get; private set; }
        public decimal? preco_custo_he { get; private set; }
        public decimal? preco_primera_he { get; private set; }
        public decimal? preco_demais_he { get; private set; }
        public decimal? preco_custo_had { get; private set; }
        public decimal? preco_primera_had { get; private set; }
        public decimal? preco_demais_had { get; private set; }
        public int? id_config_tributaria { get; private set; }
        public string? desc_config_tributaria { get; private set; }
        public string? cod_trib_servico_municipio { get; private set; }
        public long timestamp { get; private set; }

        public LinxServicosDetalhes() { }

        public LinxServicosDetalhes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_servico = MicrovixConverters.ToInt64(dto.cod_servico);
            preco_custo_hn = MicrovixConverters.ToNullableDecimal(dto.preco_custo_hn);
            preco_primera_hn = MicrovixConverters.ToNullableDecimal(dto.preco_primera_hn);
            preco_demais_hn = MicrovixConverters.ToNullableDecimal(dto.preco_demais_hn);
            preco_custo_he = MicrovixConverters.ToNullableDecimal(dto.preco_custo_he);
            preco_primera_he = MicrovixConverters.ToNullableDecimal(dto.preco_primera_he);
            preco_demais_he = MicrovixConverters.ToNullableDecimal(dto.preco_demais_he);
            preco_custo_had = MicrovixConverters.ToNullableDecimal(dto.preco_custo_had);
            preco_primera_had = MicrovixConverters.ToNullableDecimal(dto.preco_primera_had);
            preco_demais_had = MicrovixConverters.ToNullableDecimal(dto.preco_demais_had);
            id_config_tributaria = MicrovixConverters.ToNullableInt32(dto.id_config_tributaria);
            desc_config_tributaria = MicrovixConverters.ToNullableString(dto.desc_config_tributaria, 100);
            cod_trib_servico_municipio = MicrovixConverters.ToNullableString(dto.cod_trib_servico_municipio, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
