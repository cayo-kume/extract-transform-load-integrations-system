using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoObservacoes, tabela linxmicrovix.LinxMovimentoObservacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_obs (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoObservacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_obs { get; private set; }
        public string? titulo_obs { get; private set; }
        public string? descricao_obs { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoObservacoes() { }

        public LinxMovimentoObservacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoObservacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_obs = MicrovixConverters.ToInt32(dto.id_obs);
            titulo_obs = MicrovixConverters.ToNullableString(dto.titulo_obs, 50);
            descricao_obs = MicrovixConverters.ToNullableString(dto.descricao_obs, 250);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
