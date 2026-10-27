using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosCodBar, tabela linxmicrovix.LinxProdutosCodBar. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_produto + cod_barra
    /// </summary>
    public class LinxProdutosCodBar
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public long cod_produto { get; private set; }
        public string cod_barra { get; private set; } = String.Empty;
        public long timestamp { get; private set; }

        public LinxProdutosCodBar() { }

        public LinxProdutosCodBar(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosCodBar dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            cod_barra = MicrovixConverters.ToNullableString(dto.cod_barra, 20) ?? String.Empty;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
