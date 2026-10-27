using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxColecoes, tabela linxmicrovix.LinxColecoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_colecao
    /// </summary>
    public class LinxColecoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_colecao { get; private set; }
        public string? desc_colecao { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public int portal { get; private set; }

        public LinxColecoes() { }

        public LinxColecoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxColecoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_colecao = MicrovixConverters.ToInt32(dto.id_colecao);
            desc_colecao = MicrovixConverters.ToNullableString(dto.desc_colecao, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
