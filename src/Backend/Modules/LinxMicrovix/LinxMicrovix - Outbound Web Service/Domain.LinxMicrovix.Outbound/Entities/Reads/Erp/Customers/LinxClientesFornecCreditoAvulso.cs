using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornecCreditoAvulso, tabela linxmicrovix.LinxClientesFornecCreditoAvulso. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_cliente + controle (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxClientesFornecCreditoAvulso
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int cod_cliente { get; private set; }
        public int controle { get; private set; }
        public DateTime? data { get; private set; }
        public string? cd { get; private set; }
        public decimal? valor { get; private set; }
        public string? motivo { get; private set; }
        public long timestamp { get; private set; }
        public Guid? identificador { get; private set; }

        public LinxClientesFornecCreditoAvulso() { }

        public LinxClientesFornecCreditoAvulso(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            controle = MicrovixConverters.ToInt32(dto.controle);
            data = MicrovixConverters.ToNullableDateTime(dto.data);
            cd = MicrovixConverters.ToNullableString(dto.cd, 1);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            motivo = String.IsNullOrEmpty(dto.motivo) ? null : dto.motivo;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
        }
    }
}
