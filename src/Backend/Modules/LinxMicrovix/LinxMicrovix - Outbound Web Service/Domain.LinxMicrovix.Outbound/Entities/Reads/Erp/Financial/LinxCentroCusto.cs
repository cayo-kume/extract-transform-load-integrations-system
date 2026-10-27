using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxCentroCusto, tabela linxmicrovix.LinxCentroCusto. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + empresa + id_centrocusto
    /// </summary>
    public class LinxCentroCusto
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int empresa { get; private set; }
        public string? CNPJ { get; private set; }
        public int id_centrocusto { get; private set; }
        public string? nome_centrocusto { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }

        public LinxCentroCusto() { }

        public LinxCentroCusto(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxCentroCusto dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            CNPJ = MicrovixConverters.ToNullableString(dto.CNPJ, 14);
            id_centrocusto = MicrovixConverters.ToInt32(dto.id_centrocusto);
            nome_centrocusto = MicrovixConverters.ToNullableString(dto.nome_centrocusto, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
