using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornecContatos, tabela linxmicrovix.LinxClientesFornecContatos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_cliente + nome_contato + tipo_contato
    /// </summary>
    public class LinxClientesFornecContatos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int cod_cliente { get; private set; }
        public string nome_contato { get; private set; } = String.Empty;
        public string? sexo_contato { get; private set; }
        public int? contatos_clientes_parentesco { get; private set; }
        public string? fone1_contato { get; private set; }
        public string? fone2_contato { get; private set; }
        public string? celular_contato { get; private set; }
        public string? email_contato { get; private set; }
        public DateTime? data_nasc_contato { get; private set; }
        public string tipo_contato { get; private set; } = String.Empty;
        public long timestamp { get; private set; }

        public LinxClientesFornecContatos() { }

        public LinxClientesFornecContatos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornecContatos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            nome_contato = MicrovixConverters.ToNullableString(dto.nome_contato, 50) ?? String.Empty;
            sexo_contato = MicrovixConverters.ToNullableString(dto.sexo_contato, 1);
            contatos_clientes_parentesco = MicrovixConverters.ToNullableInt32(dto.contatos_clientes_parentesco);
            fone1_contato = MicrovixConverters.ToNullableString(dto.fone1_contato, 20);
            fone2_contato = MicrovixConverters.ToNullableString(dto.fone2_contato, 20);
            celular_contato = MicrovixConverters.ToNullableString(dto.celular_contato, 20);
            email_contato = MicrovixConverters.ToNullableString(dto.email_contato, 50);
            data_nasc_contato = MicrovixConverters.ToNullableDateTime(dto.data_nasc_contato);
            tipo_contato = MicrovixConverters.ToNullableString(dto.tipo_contato, 20) ?? String.Empty;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
