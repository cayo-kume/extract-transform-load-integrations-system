using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxRemessasOperacoes, tabela linxmicrovix.LinxRemessasOperacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_remessa_operacoes (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRemessasOperacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_remessa_operacoes { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxRemessasOperacoes() { }

        public LinxRemessasOperacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxRemessasOperacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_remessa_operacoes = MicrovixConverters.ToInt32(dto.id_remessa_operacoes);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
