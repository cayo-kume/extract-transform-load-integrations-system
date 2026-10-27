using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxFreteTipos, tabela linxmicrovix.LinxFreteTipos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_frete_tipos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxFreteTipos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_frete_tipos { get; private set; }
        public string? descricao { get; private set; }
        public int? codigo_sped { get; private set; }
        public int? codigo_nfe { get; private set; }
        public int? codigo_sintegra { get; private set; }
        public bool? permite_valor { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxFreteTipos() { }

        public LinxFreteTipos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxFreteTipos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_frete_tipos = MicrovixConverters.ToInt32(dto.id_frete_tipos);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 40);
            codigo_sped = MicrovixConverters.ToNullableInt32(dto.codigo_sped);
            codigo_nfe = MicrovixConverters.ToNullableInt32(dto.codigo_nfe);
            codigo_sintegra = MicrovixConverters.ToNullableInt32(dto.codigo_sintegra);
            permite_valor = MicrovixConverters.ToNullableBoolean(dto.permite_valor);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
