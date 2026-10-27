using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaTags, tabela linxmicrovix.B2CConsultaTags. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_tags (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaTags
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_b2c_tags { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public B2CConsultaTags() { }

        public B2CConsultaTags(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaTags dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_b2c_tags = MicrovixConverters.ToInt32(dto.id_b2c_tags);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 300);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
