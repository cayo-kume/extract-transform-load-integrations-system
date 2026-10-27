using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaStatus, tabela linxmicrovix.B2CConsultaStatus. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_status (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_status { get; private set; }
        public string? descricao_status { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaStatus() { }

        public B2CConsultaStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_status = MicrovixConverters.ToInt32(dto.id_status);
            descricao_status = String.IsNullOrEmpty(dto.descricao_status) ? null : dto.descricao_status;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
