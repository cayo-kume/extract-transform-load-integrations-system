using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosInformacoes, tabela linxmicrovix.LinxProdutosInformacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigoproduto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosInformacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public string? informacoes_produto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxProdutosInformacoes() { }

        public LinxProdutosInformacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosInformacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            informacoes_produto = String.IsNullOrEmpty(dto.informacoes_produto) ? null : dto.informacoes_produto;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
