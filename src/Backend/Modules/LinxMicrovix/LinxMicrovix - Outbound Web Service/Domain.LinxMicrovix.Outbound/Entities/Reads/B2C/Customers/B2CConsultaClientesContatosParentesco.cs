using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientesContatosParentesco, tabela linxmicrovix.B2CConsultaClientesContatosParentesco. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_parentesco (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaClientesContatosParentesco
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_parentesco { get; private set; }
        public string? descricao_parentesco { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaClientesContatosParentesco() { }

        public B2CConsultaClientesContatosParentesco(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_parentesco = MicrovixConverters.ToInt32(dto.id_parentesco);
            descricao_parentesco = String.IsNullOrEmpty(dto.descricao_parentesco) ? null : dto.descricao_parentesco;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
