using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxTradeinParceiro, tabela linxmicrovix.LinxTradeinParceiro. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tradein_parceiro (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxTradeinParceiro
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_tradein_parceiro { get; private set; }
        public string? nome_parceiro { get; private set; }
        public long timestamp { get; private set; }

        public LinxTradeinParceiro() { }

        public LinxTradeinParceiro(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxTradeinParceiro dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_tradein_parceiro = MicrovixConverters.ToInt32(dto.id_tradein_parceiro);
            nome_parceiro = MicrovixConverters.ToNullableString(dto.nome_parceiro, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
