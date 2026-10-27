using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxSubClasses, tabela linxmicrovix.LinxSubClasses. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_subclasses (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxSubClasses
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_subclasses { get; private set; }
        public int? classe { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxSubClasses() { }

        public LinxSubClasses(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxSubClasses dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_subclasses = MicrovixConverters.ToInt32(dto.id_subclasses);
            classe = MicrovixConverters.ToNullableInt32(dto.classe);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
