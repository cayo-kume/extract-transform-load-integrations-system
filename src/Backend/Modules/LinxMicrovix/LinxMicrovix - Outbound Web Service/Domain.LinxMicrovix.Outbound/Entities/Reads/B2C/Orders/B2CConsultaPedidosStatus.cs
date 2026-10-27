using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaPedidosStatus, tabela linxmicrovix.B2CConsultaPedidosStatus. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id
    /// </summary>
    public class B2CConsultaPedidosStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id { get; private set; }
        public int? id_status { get; private set; }
        public int? id_pedido { get; private set; }
        public DateTime? data_hora { get; private set; }
        public string? anotacao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaPedidosStatus() { }

        public B2CConsultaPedidosStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = MicrovixConverters.ToInt32(dto.id);
            id_status = MicrovixConverters.ToNullableInt32(dto.id_status);
            id_pedido = MicrovixConverters.ToNullableInt32(dto.id_pedido);
            data_hora = MicrovixConverters.ToNullableDateTime(dto.data_hora);
            anotacao = MicrovixConverters.ToNullableString(dto.anotacao, 80);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
