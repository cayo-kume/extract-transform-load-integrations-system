using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosImagensURL, tabela linxmicrovix.LinxProdutosImagensURL. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_imagem + codigoproduto
    /// </summary>
    public class LinxProdutosImagensURL
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public string? url_imagem_blob { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }
        public int id_imagem { get; private set; }

        public LinxProdutosImagensURL() { }

        public LinxProdutosImagensURL(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosImagensURL dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            url_imagem_blob = MicrovixConverters.ToNullableString(dto.url_imagem_blob, 200);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
            id_imagem = MicrovixConverters.ToInt32(dto.id_imagem);
        }
    }
}
