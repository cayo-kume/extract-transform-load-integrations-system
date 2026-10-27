using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosDimensoes, tabela linxmicrovix.B2CConsultaProdutosDimensoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosDimensoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public decimal? altura { get; private set; }
        public decimal? comprimento { get; private set; }
        public long timestamp { get; private set; }
        public decimal? largura { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosDimensoes() { }

        public B2CConsultaProdutosDimensoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosDimensoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            altura = MicrovixConverters.ToNullableDecimal(dto.altura);   //fora da documentação
            comprimento = MicrovixConverters.ToNullableDecimal(dto.comprimento);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            largura = MicrovixConverters.ToNullableDecimal(dto.largura);   //fora da documentação
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
