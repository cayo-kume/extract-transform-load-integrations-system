using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientesEstadoCivil, tabela linxmicrovix.B2CConsultaClientesEstadoCivil. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_estado_civil (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaClientesEstadoCivil
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_estado_civil { get; private set; }
        public string? estado_civil { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaClientesEstadoCivil() { }

        public B2CConsultaClientesEstadoCivil(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_estado_civil = MicrovixConverters.ToInt32(dto.id_estado_civil);
            estado_civil = MicrovixConverters.ToNullableString(dto.estado_civil, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
