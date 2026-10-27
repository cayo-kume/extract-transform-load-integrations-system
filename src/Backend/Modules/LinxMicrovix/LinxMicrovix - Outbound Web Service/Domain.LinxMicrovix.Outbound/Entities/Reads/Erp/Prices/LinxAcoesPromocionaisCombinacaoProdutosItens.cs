using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxAcoesPromocionaisCombinacaoProdutosItens, tabela linxmicrovix.LinxAcoesPromocionaisCombinacaoProdutosItens. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_acoes_promocionais_combinacao_produtos_itens
    /// </summary>
    public class LinxAcoesPromocionaisCombinacaoProdutosItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_acoes_promocionais_combinacao_produtos_itens { get; private set; }
        public int? id_combinacao_produto { get; private set; }
        public long? codigoproduto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxAcoesPromocionaisCombinacaoProdutosItens() { }

        public LinxAcoesPromocionaisCombinacaoProdutosItens(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxAcoesPromocionaisCombinacaoProdutosItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_acoes_promocionais_combinacao_produtos_itens = MicrovixConverters.ToInt32(dto.id_acoes_promocionais_combinacao_produtos_itens);
            id_combinacao_produto = MicrovixConverters.ToNullableInt32(dto.id_combinacao_produto);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
