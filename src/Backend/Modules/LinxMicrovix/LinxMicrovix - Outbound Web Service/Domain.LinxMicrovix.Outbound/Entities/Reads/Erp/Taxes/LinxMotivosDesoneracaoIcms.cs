using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxMotivosDesoneracaoIcms, tabela linxmicrovix.LinxMotivosDesoneracaoIcms. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_motivos_desoneracao_icms (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMotivosDesoneracaoIcms
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_motivos_desoneracao_icms { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxMotivosDesoneracaoIcms() { }

        public LinxMotivosDesoneracaoIcms(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxMotivosDesoneracaoIcms dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_motivos_desoneracao_icms = MicrovixConverters.ToInt32(dto.id_motivos_desoneracao_icms);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
