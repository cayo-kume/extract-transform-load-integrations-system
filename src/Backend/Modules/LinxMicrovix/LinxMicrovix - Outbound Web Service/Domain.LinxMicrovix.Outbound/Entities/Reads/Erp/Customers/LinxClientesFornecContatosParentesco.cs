using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornecContatosParentesco, tabela linxmicrovix.LinxClientesFornecContatosParentesco. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_parentesco (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxClientesFornecContatosParentesco
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_parentesco { get; private set; }
        public string? descricao_parentesco { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxClientesFornecContatosParentesco() { }

        public LinxClientesFornecContatosParentesco(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornecContatosParentesco dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_parentesco = MicrovixConverters.ToInt32(dto.id_parentesco);
            descricao_parentesco = MicrovixConverters.ToNullableString(dto.descricao_parentesco, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
