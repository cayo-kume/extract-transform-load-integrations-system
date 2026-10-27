using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientesSaldo, tabela linxmicrovix.B2CConsultaClientesSaldo. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + cod_cliente_erp + empresa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaClientesSaldo
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public decimal? saldo { get; private set; }
        public int cod_cliente_erp { get; private set; }
        public int empresa { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaClientesSaldo() { }

        public B2CConsultaClientesSaldo(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            saldo = MicrovixConverters.ToNullableDecimal(dto.saldo);
            cod_cliente_erp = MicrovixConverters.ToInt32(dto.cod_cliente_erp);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
