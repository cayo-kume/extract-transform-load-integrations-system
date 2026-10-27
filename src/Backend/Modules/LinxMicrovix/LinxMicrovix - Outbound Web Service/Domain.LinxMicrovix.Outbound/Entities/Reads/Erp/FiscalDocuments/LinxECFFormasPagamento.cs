using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxECFFormasPagamento, tabela linxmicrovix.LinxECFFormasPagamento. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_empresa_ecf_formas_pgto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxECFFormasPagamento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_empresa_ecf_formas_pgto { get; private set; }
        public int? id_ecf { get; private set; }
        public int? cod_forma_pgto { get; private set; }
        public string? indice_forma { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxECFFormasPagamento() { }

        public LinxECFFormasPagamento(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxECFFormasPagamento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_empresa_ecf_formas_pgto = MicrovixConverters.ToInt32(dto.id_empresa_ecf_formas_pgto);
            id_ecf = MicrovixConverters.ToNullableInt32(dto.id_ecf);
            cod_forma_pgto = MicrovixConverters.ToNullableInt32(dto.cod_forma_pgto);
            indice_forma = MicrovixConverters.ToNullableString(dto.indice_forma, 53);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
