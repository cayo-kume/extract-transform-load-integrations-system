using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosInformacoes, tabela linxmicrovix.B2CConsultaProdutosInformacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_produtos_informacoes (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosInformacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_produtos_informacoes { get; private set; }
        public long? codigoproduto { get; private set; }
        public string? informacoes_produto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosInformacoes() { }

        public B2CConsultaProdutosInformacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosInformacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_produtos_informacoes = MicrovixConverters.ToInt32(dto.id_produtos_informacoes);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            informacoes_produto = String.IsNullOrEmpty(dto.informacoes_produto) ? null : dto.informacoes_produto;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
