using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornecClasses, tabela linxmicrovix.LinxClientesFornecClasses. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_cliente + cod_classe (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxClientesFornecClasses
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_cliente { get; private set; }
        public int cod_classe { get; private set; }
        public string? nome_classe { get; private set; }
        public long timestamp { get; private set; }

        public LinxClientesFornecClasses() { }

        public LinxClientesFornecClasses(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornecClasses dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            cod_classe = MicrovixConverters.ToInt32(dto.cod_classe);
            nome_classe = MicrovixConverters.ToNullableString(dto.nome_classe, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
