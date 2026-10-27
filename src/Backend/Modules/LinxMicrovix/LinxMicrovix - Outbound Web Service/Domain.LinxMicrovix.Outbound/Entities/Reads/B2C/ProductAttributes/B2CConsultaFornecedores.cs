using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaFornecedores, tabela linxmicrovix.B2CConsultaFornecedores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_fornecedor
    /// </summary>
    public class B2CConsultaFornecedores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int cod_fornecedor { get; private set; }
        public string? nome { get; private set; }
        public string? nome_fantasia { get; private set; }
        public string? tipo_pessoa { get; private set; }
        public string? tipo_fornecedor { get; private set; }
        public string? endereco { get; private set; }
        public string? numero_rua { get; private set; }
        public string? bairro { get; private set; }
        public string? cep { get; private set; }
        public string? cidade { get; private set; }
        public string? uf { get; private set; }
        public string? documento { get; private set; }
        public string? fone { get; private set; }
        public string? email { get; private set; }
        public string? pais { get; private set; }
        public string? obs { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaFornecedores() { }

        public B2CConsultaFornecedores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaFornecedores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_fornecedor = MicrovixConverters.ToInt32(dto.cod_fornecedor);
            nome = MicrovixConverters.ToNullableString(dto.nome, 250);   //fora da documentação
            nome_fantasia = MicrovixConverters.ToNullableString(dto.nome_fantasia, 60);
            tipo_pessoa = MicrovixConverters.ToNullableString(dto.tipo_pessoa, 1);
            tipo_fornecedor = MicrovixConverters.ToNullableString(dto.tipo_fornecedor, 1);
            endereco = MicrovixConverters.ToNullableString(dto.endereco, 250);
            numero_rua = MicrovixConverters.ToNullableString(dto.numero_rua, 20);
            bairro = MicrovixConverters.ToNullableString(dto.bairro, 60);
            cep = MicrovixConverters.ToNullableString(dto.cep, 9);
            cidade = MicrovixConverters.ToNullableString(dto.cidade, 40);
            uf = MicrovixConverters.ToNullableString(dto.uf, 200);
            documento = MicrovixConverters.ToNullableString(dto.documento, 14);
            fone = MicrovixConverters.ToNullableString(dto.fone, 20);
            email = MicrovixConverters.ToNullableString(dto.email, 50);
            pais = MicrovixConverters.ToNullableString(dto.pais, 80);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
