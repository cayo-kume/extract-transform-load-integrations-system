using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosTags, tabela linxmicrovix.B2CConsultaProdutosTags. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_tags_produtos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosTags
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_b2c_tags_produtos { get; private set; }
        public int? id_b2c_tags { get; private set; }
        public long? codigoproduto { get; private set; }
        public long timestamp { get; private set; }
        public string? descricao_b2c_tags { get; private set; }

        public B2CConsultaProdutosTags() { }

        public B2CConsultaProdutosTags(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosTags dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_b2c_tags_produtos = MicrovixConverters.ToInt32(dto.id_b2c_tags_produtos);
            id_b2c_tags = MicrovixConverters.ToNullableInt32(dto.id_b2c_tags);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            descricao_b2c_tags = MicrovixConverters.ToNullableString(dto.descricao_b2c_tags, 300);
        }
    }
}
