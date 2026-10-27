using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaCodigoRastreio, tabela linxmicrovix.B2CConsultaCodigoRastreio. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_pedido + codigo_rastreio
    /// </summary>
    public class B2CConsultaCodigoRastreio
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_pedido { get; private set; }
        public int? documento { get; private set; }
        public string? serie { get; private set; }
        public string codigo_rastreio { get; private set; } = String.Empty;
        public string? sequencia_volume { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaCodigoRastreio() { }

        public B2CConsultaCodigoRastreio(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaCodigoRastreio dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_pedido = MicrovixConverters.ToInt32(dto.id_pedido);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            codigo_rastreio = MicrovixConverters.ToNullableString(dto.codigo_rastreio, 20) ?? String.Empty;
            sequencia_volume = MicrovixConverters.ToNullableString(dto.sequencia_volume, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
