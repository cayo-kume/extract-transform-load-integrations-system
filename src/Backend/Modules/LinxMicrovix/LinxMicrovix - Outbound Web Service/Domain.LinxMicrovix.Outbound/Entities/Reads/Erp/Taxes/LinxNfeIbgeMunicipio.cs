using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxNfeIbgeMunicipio, tabela linxmicrovix.LinxNfeIbgeMunicipio. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_nfe_ibge_municipio
    /// </summary>
    public class LinxNfeIbgeMunicipio
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_nfe_ibge_municipio { get; private set; }
        public int? cod_ibge_municipio { get; private set; }
        public string? nome { get; private set; }
        public int? cod_ibge_uf { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxNfeIbgeMunicipio() { }

        public LinxNfeIbgeMunicipio(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxNfeIbgeMunicipio dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_nfe_ibge_municipio = MicrovixConverters.ToInt32(dto.id_nfe_ibge_municipio);
            cod_ibge_municipio = MicrovixConverters.ToNullableInt32(dto.cod_ibge_municipio);
            nome = MicrovixConverters.ToNullableString(dto.nome, 100);   //fora da documentação
            cod_ibge_uf = MicrovixConverters.ToNullableInt32(dto.cod_ibge_uf);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
