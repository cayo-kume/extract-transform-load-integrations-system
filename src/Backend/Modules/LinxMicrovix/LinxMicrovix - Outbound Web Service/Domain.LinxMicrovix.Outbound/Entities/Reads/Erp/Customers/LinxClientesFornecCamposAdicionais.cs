using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornecCamposAdicionais, tabela linxmicrovix.LinxClientesFornecCamposAdicionais. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_cliente + campo (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxClientesFornecCamposAdicionais
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_cliente { get; private set; }
        public string campo { get; private set; } = String.Empty;
        public string? valor { get; private set; }
        public long timestamp { get; private set; }

        public LinxClientesFornecCamposAdicionais() { }

        public LinxClientesFornecCamposAdicionais(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            campo = MicrovixConverters.ToNullableString(dto.campo, 1000) ?? String.Empty;   //fora da documentação
            valor = MicrovixConverters.ToNullableString(dto.valor, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
