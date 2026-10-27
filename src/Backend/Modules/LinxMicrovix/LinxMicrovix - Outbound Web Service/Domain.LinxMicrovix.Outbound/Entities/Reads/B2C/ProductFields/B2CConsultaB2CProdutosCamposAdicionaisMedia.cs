using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaB2CProdutosCamposAdicionaisMedia, tabela linxmicrovix.B2CConsultaB2CProdutosCamposAdicionaisMedia. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_produtos_campos_adicionais_media (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisMedia
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_b2c_produtos_campos_adicionais_media { get; private set; }
        public string? descricao { get; private set; }
        public string? url { get; private set; }
        public string? tipo_media_descricao { get; private set; }
        public int? portal { get; private set; }
        public bool? media_principal { get; private set; }
        public int? tipo_media_id { get; private set; }
        public long timestamp { get; private set; }

        public B2CConsultaB2CProdutosCamposAdicionaisMedia() { }

        public B2CConsultaB2CProdutosCamposAdicionaisMedia(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_b2c_produtos_campos_adicionais_media = MicrovixConverters.ToInt32(dto.id_b2c_produtos_campos_adicionais_media);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            url = MicrovixConverters.ToNullableString(dto.url, 400);
            tipo_media_descricao = MicrovixConverters.ToNullableString(dto.tipo_media_descricao, 50);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            media_principal = MicrovixConverters.ToNullableBoolean(dto.media_principal);
            tipo_media_id = MicrovixConverters.ToNullableInt32(dto.tipo_media_id);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
