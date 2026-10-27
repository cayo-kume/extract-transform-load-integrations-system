using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxValesComprasEnviadosAPI, tabela linxmicrovix.LinxValesComprasEnviadosAPI. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + numero_controle (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxValesComprasEnviadosAPI
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int numero_controle { get; private set; }
        public decimal? valor_vale { get; private set; }
        public string? doc_cliente { get; private set; }
        public int? status_vale { get; private set; }
        public int? codigo_portal_resgate { get; private set; }
        public int? codigo_empresa_resgate { get; private set; }
        public int? codigo_usuario_resgate { get; private set; }
        public int? codigo_vale_empresa_resgate { get; private set; }
        public DateTime? data_criacao { get; private set; }
        public long timestamp { get; private set; }
        public string? cnpj_empresa_resgate { get; private set; }
        public DateTime? data_resgate { get; private set; }
        public int? portal { get; private set; }

        public LinxValesComprasEnviadosAPI() { }

        public LinxValesComprasEnviadosAPI(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxValesComprasEnviadosAPI dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            numero_controle = MicrovixConverters.ToInt32(dto.numero_controle);
            valor_vale = MicrovixConverters.ToNullableDecimal(dto.valor_vale);
            doc_cliente = MicrovixConverters.ToNullableString(dto.doc_cliente, 14);
            status_vale = MicrovixConverters.ToNullableInt32(dto.status_vale);
            codigo_portal_resgate = MicrovixConverters.ToNullableInt32(dto.codigo_portal_resgate);
            codigo_empresa_resgate = MicrovixConverters.ToNullableInt32(dto.codigo_empresa_resgate);
            codigo_usuario_resgate = MicrovixConverters.ToNullableInt32(dto.codigo_usuario_resgate);
            codigo_vale_empresa_resgate = MicrovixConverters.ToNullableInt32(dto.codigo_vale_empresa_resgate);
            data_criacao = MicrovixConverters.ToNullableDateTime(dto.data_criacao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            cnpj_empresa_resgate = MicrovixConverters.ToNullableString(dto.cnpj_empresa_resgate, 14);
            data_resgate = MicrovixConverters.ToNullableDateTime(dto.data_resgate);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
