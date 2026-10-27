using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxNfse, tabela linxmicrovix.LinxNfse. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_nfse (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxNfse
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public string? portal { get; private set; }
        public string? documento { get; private set; }
        public int id_nfse { get; private set; }
        public string? codigo_verificacao { get; private set; }
        public int? id_nfse_situacao { get; private set; }
        public Guid? identificador { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public long timestamp { get; private set; }

        public LinxNfse() { }

        public LinxNfse(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxNfse dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableString(dto.portal, 20);
            documento = MicrovixConverters.ToNullableString(dto.documento, 20);
            id_nfse = MicrovixConverters.ToInt32(dto.id_nfse);
            codigo_verificacao = MicrovixConverters.ToNullableString(dto.codigo_verificacao, 50);
            id_nfse_situacao = MicrovixConverters.ToNullableInt32(dto.id_nfse_situacao);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
