using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages
{
    /// <summary>
    /// B2CConsultaProdutosImagensURL, tabela linxmicrovix.B2CConsultaProdutosImagensURL. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + codigoproduto + url_imagem_blob (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosImagensURL
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public string url_imagem_blob { get; private set; } = String.Empty;
        public int? tipo { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaProdutosImagensURL() { }

        public B2CConsultaProdutosImagensURL(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            url_imagem_blob = MicrovixConverters.ToNullableString(dto.url_imagem_blob, 200) ?? String.Empty;
            tipo = MicrovixConverters.ToNullableInt32(dto.tipo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
