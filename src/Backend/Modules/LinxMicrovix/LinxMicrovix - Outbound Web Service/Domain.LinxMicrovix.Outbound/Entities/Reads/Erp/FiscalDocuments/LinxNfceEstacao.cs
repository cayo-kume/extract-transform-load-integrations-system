using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxNfceEstacao, tabela linxmicrovix.LinxNfceEstacao. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_nfce_estacao (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxNfceEstacao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_nfce_estacao { get; private set; }
        public int? empresa { get; private set; }
        public string? descricao { get; private set; }
        public string? numero_pdv_tef { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxNfceEstacao() { }

        public LinxNfceEstacao(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxNfceEstacao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_nfce_estacao = MicrovixConverters.ToInt32(dto.id_nfce_estacao);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            numero_pdv_tef = MicrovixConverters.ToNullableString(dto.numero_pdv_tef, 20);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
