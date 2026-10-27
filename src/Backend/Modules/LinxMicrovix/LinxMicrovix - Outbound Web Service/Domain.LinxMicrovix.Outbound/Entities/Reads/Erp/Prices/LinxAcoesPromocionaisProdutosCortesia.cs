using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxAcoesPromocionaisProdutosCortesia, tabela linxmicrovix.LinxAcoesPromocionaisProdutosCortesia. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_acoes_promocionais_produtos_cortesia
    /// </summary>
    public class LinxAcoesPromocionaisProdutosCortesia
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_acoes_promocionais_produtos_cortesia { get; private set; }
        public int? id_acoes_promocionais { get; private set; }
        public long? codigoproduto { get; private set; }
        public int? id_combinacao_produto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxAcoesPromocionaisProdutosCortesia() { }

        public LinxAcoesPromocionaisProdutosCortesia(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxAcoesPromocionaisProdutosCortesia dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_acoes_promocionais_produtos_cortesia = MicrovixConverters.ToInt32(dto.id_acoes_promocionais_produtos_cortesia);
            id_acoes_promocionais = MicrovixConverters.ToNullableInt32(dto.id_acoes_promocionais);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            id_combinacao_produto = MicrovixConverters.ToNullableInt32(dto.id_combinacao_produto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
