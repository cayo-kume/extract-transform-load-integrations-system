using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaPedidosIdentificador, tabela linxmicrovix.B2CConsultaPedidosIdentificador. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + identificador
    /// </summary>
    public class B2CConsultaPedidosIdentificador
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int? empresa { get; private set; }
        public Guid identificador { get; private set; }
        public int? id_venda { get; private set; }
        public string? order_id { get; private set; }
        public int? id_cliente { get; private set; }
        public decimal? valor_frete { get; private set; }
        public DateTime? data_origem { get; private set; }
        public long timestamp { get; private set; }

        public B2CConsultaPedidosIdentificador() { }

        public B2CConsultaPedidosIdentificador(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaPedidosIdentificador dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            id_venda = MicrovixConverters.ToNullableInt32(dto.id_venda);
            order_id = MicrovixConverters.ToNullableString(dto.order_id, 40);
            id_cliente = MicrovixConverters.ToNullableInt32(dto.id_cliente);
            valor_frete = MicrovixConverters.ToNullableDecimal(dto.valor_frete);
            data_origem = MicrovixConverters.ToNullableDateTime(dto.data_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
