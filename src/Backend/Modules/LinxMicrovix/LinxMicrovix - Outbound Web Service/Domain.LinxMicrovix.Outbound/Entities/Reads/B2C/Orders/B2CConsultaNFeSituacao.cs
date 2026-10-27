using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaNFeSituacao, tabela linxmicrovix.B2CConsultaNFeSituacao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_nfe_situacao (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaNFeSituacao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_nfe_situacao { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaNFeSituacao() { }

        public B2CConsultaNFeSituacao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaNFeSituacao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_nfe_situacao = MicrovixConverters.ToInt32(dto.id_nfe_situacao);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
