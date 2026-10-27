using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosPalavrasChavePesquisa, tabela linxmicrovix.B2CConsultaProdutosPalavrasChavePesquisa. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_palavras_chave_pesquisa_produtos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosPalavrasChavePesquisa
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_b2c_palavras_chave_pesquisa_produtos { get; private set; }
        public int? id_b2c_palavras_chave_pesquisa { get; private set; }
        public long? codigoproduto { get; private set; }
        public long timestamp { get; private set; }
        public string? descricao_b2c_palavras_chave_pesquisa { get; private set; }

        public B2CConsultaProdutosPalavrasChavePesquisa() { }

        public B2CConsultaProdutosPalavrasChavePesquisa(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosPalavrasChavePesquisa dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_b2c_palavras_chave_pesquisa_produtos = MicrovixConverters.ToInt32(dto.id_b2c_palavras_chave_pesquisa_produtos);
            id_b2c_palavras_chave_pesquisa = MicrovixConverters.ToNullableInt32(dto.id_b2c_palavras_chave_pesquisa);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            descricao_b2c_palavras_chave_pesquisa = String.IsNullOrEmpty(dto.descricao_b2c_palavras_chave_pesquisa) ? null : dto.descricao_b2c_palavras_chave_pesquisa;
        }
    }
}
