using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages
{
    /// <summary>
    /// B2CConsultaImagensHD, tabela linxmicrovix.B2CConsultaImagensHD. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + codigoproduto + identificador_imagem (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaImagensHD
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador_imagem { get; private set; }
        public long codigoproduto { get; private set; }
        public string? imagem { get; private set; }
        public long timestamp { get; private set; }
        public string? url_imagem_blob { get; private set; }

        public B2CConsultaImagensHD() { }

        public B2CConsultaImagensHD(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductImages.B2CConsultaImagensHD dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador_imagem = MicrovixConverters.ToNullableGuid(dto.identificador_imagem) ?? Guid.Empty;
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            imagem = MicrovixConverters.ToNullableString(dto.imagem, 1000);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            url_imagem_blob = MicrovixConverters.ToNullableString(dto.url_imagem_blob, 200);
        }
    }
}
