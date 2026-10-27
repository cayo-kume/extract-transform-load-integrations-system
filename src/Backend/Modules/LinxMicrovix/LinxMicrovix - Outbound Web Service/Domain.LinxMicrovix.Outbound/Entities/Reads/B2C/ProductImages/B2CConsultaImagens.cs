using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages
{
    /// <summary>
    /// B2CConsultaImagens, tabela linxmicrovix.B2CConsultaImagens. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_imagem (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaImagens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_imagem { get; private set; }
        public string? imagem { get; private set; }
        public string? md5 { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }
        public string? url_imagem_blob { get; private set; }

        public B2CConsultaImagens() { }

        public B2CConsultaImagens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductImages.B2CConsultaImagens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_imagem = MicrovixConverters.ToInt32(dto.id_imagem);
            imagem = MicrovixConverters.ToNullableString(dto.imagem, 1000);   //fora da documentação
            md5 = String.IsNullOrEmpty(dto.md5) ? null : dto.md5;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            url_imagem_blob = MicrovixConverters.ToNullableString(dto.url_imagem_blob, 1000);   //fora da documentação
        }
    }
}
