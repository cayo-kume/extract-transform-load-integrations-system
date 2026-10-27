using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxVendedoresFerias, tabela linxmicrovix.LinxVendedoresFerias. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_vendedor + data_inicial (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxVendedoresFerias
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_vendedor { get; private set; }
        public DateTime data_inicial { get; private set; }
        public DateTime? data_final { get; private set; }
        public long timestamp { get; private set; }

        public LinxVendedoresFerias() { }

        public LinxVendedoresFerias(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxVendedoresFerias dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_vendedor = MicrovixConverters.ToInt32(dto.cod_vendedor);
            data_inicial = MicrovixConverters.ToNullableDateTime(dto.data_inicial) ?? new DateTime(1900, 1, 1);
            data_final = MicrovixConverters.ToNullableDateTime(dto.data_final);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
