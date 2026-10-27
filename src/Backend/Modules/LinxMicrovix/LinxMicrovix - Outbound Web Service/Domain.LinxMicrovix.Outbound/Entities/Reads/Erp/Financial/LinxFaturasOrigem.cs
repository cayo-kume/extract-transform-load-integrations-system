using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxFaturasOrigem, tabela linxmicrovix.LinxFaturasOrigem. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + codigo_fatura + codigo_fatura_origem (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxFaturasOrigem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long codigo_fatura { get; private set; }
        public long codigo_fatura_origem { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxFaturasOrigem() { }

        public LinxFaturasOrigem(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxFaturasOrigem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            codigo_fatura = MicrovixConverters.ToInt64(dto.codigo_fatura);
            codigo_fatura_origem = MicrovixConverters.ToInt64(dto.codigo_fatura_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
