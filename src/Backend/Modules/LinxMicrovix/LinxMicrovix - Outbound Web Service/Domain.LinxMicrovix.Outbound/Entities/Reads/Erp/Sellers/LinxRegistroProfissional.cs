using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxRegistroProfissional, tabela linxmicrovix.LinxRegistroProfissional. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_tipo_registro_profissional (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxRegistroProfissional
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_tipo_registro_profissional { get; private set; }
        public string? descricao_tipo_registro_profissional { get; private set; }
        public bool? ativo { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }

        public LinxRegistroProfissional() { }

        public LinxRegistroProfissional(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxRegistroProfissional dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_tipo_registro_profissional = MicrovixConverters.ToInt32(dto.id_tipo_registro_profissional);
            descricao_tipo_registro_profissional = MicrovixConverters.ToNullableString(dto.descricao_tipo_registro_profissional, 100);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
