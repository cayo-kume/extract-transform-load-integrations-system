using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientesContatos, tabela linxmicrovix.B2CConsultaClientesContatos. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_clientes_contatos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaClientesContatos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_clientes_contatos { get; private set; }
        public int? id_contato_b2c { get; private set; }
        public string? nome_contato { get; private set; }
        public DateTime? data_nasc_contato { get; private set; }
        public string? sexo_contato { get; private set; }
        public int? id_parentesco { get; private set; }
        public string? fone_contato { get; private set; }
        public string? celular_contato { get; private set; }
        public string? email_contato { get; private set; }
        public int? cod_cliente_erp { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaClientesContatos() { }

        public B2CConsultaClientesContatos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_clientes_contatos = MicrovixConverters.ToInt32(dto.id_clientes_contatos);
            id_contato_b2c = MicrovixConverters.ToNullableInt32(dto.id_contato_b2c);
            nome_contato = MicrovixConverters.ToNullableString(dto.nome_contato, 50);
            data_nasc_contato = MicrovixConverters.ToNullableDateTime(dto.data_nasc_contato);
            sexo_contato = MicrovixConverters.ToNullableString(dto.sexo_contato, 1);
            id_parentesco = MicrovixConverters.ToNullableInt32(dto.id_parentesco);
            fone_contato = MicrovixConverters.ToNullableString(dto.fone_contato, 20);
            celular_contato = MicrovixConverters.ToNullableString(dto.celular_contato, 20);
            email_contato = MicrovixConverters.ToNullableString(dto.email_contato, 50);
            cod_cliente_erp = MicrovixConverters.ToNullableInt32(dto.cod_cliente_erp);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
