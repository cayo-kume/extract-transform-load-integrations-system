using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCest, tabela linxmicrovix.LinxCest. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cest
    /// </summary>
    public class LinxCest
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cest { get; private set; }
        public string? descricao { get; private set; }
        public string? cest { get; private set; }
        public int? id_segmento_mercadoria_bem { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }

        public LinxCest() { }

        public LinxCest(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCest dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cest = MicrovixConverters.ToInt32(dto.id_cest);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 700);
            cest = MicrovixConverters.ToNullableString(dto.cest, 10);
            id_segmento_mercadoria_bem = MicrovixConverters.ToNullableInt32(dto.id_segmento_mercadoria_bem);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
