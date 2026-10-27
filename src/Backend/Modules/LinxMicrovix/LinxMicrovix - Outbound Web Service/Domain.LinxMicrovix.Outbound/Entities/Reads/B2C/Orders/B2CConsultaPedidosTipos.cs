using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaPedidosTipos, tabela linxmicrovix.B2CConsultaPedidosTipos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tipo_b2c (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaPedidosTipos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_tipo_b2c { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaPedidosTipos() { }

        public B2CConsultaPedidosTipos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaPedidosTipos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_tipo_b2c = MicrovixConverters.ToInt32(dto.id_tipo_b2c);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
