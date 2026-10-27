using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxRemessasRetornoTipos, tabela linxmicrovix.LinxRemessasRetornoTipos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_remessa_retorno_tipos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRemessasRetornoTipos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_remessa_retorno_tipos { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxRemessasRetornoTipos() { }

        public LinxRemessasRetornoTipos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxRemessasRetornoTipos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_remessa_retorno_tipos = MicrovixConverters.ToInt32(dto.id_remessa_retorno_tipos);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
