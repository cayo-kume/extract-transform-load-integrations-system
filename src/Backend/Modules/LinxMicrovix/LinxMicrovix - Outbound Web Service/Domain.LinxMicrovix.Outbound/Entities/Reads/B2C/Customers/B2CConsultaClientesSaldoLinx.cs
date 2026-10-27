using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientesSaldoLinx, tabela linxmicrovix.B2CConsultaClientesSaldoLinx. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + cod_cliente_erp + empresa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaClientesSaldoLinx
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int cod_cliente_erp { get; private set; }
        public int? cod_cliente_b2c { get; private set; }
        public int empresa { get; private set; }
        public decimal? valor { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaClientesSaldoLinx() { }

        public B2CConsultaClientesSaldoLinx(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            cod_cliente_erp = MicrovixConverters.ToInt32(dto.cod_cliente_erp);
            cod_cliente_b2c = MicrovixConverters.ToNullableInt32(dto.cod_cliente_b2c);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
