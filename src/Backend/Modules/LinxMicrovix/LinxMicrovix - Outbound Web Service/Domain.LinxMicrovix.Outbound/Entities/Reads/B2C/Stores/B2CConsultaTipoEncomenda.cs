using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaTipoEncomenda, tabela linxmicrovix.B2CConsultaTipoEncomenda. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tipo_encomenda (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaTipoEncomenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_tipo_encomenda { get; private set; }
        public string? nm_tipo_encomenda { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaTipoEncomenda() { }

        public B2CConsultaTipoEncomenda(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaTipoEncomenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_tipo_encomenda = MicrovixConverters.ToInt32(dto.id_tipo_encomenda);
            nm_tipo_encomenda = MicrovixConverters.ToNullableString(dto.nm_tipo_encomenda, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
