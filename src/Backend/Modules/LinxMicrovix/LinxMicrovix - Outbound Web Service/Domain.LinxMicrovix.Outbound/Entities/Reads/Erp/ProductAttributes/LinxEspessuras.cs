using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxEspessuras, tabela linxmicrovix.LinxEspessuras. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_espessura
    /// </summary>
    public class LinxEspessuras
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_espessura { get; private set; }
        public string? desc_espessura { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public bool? ativo { get; private set; }
        public int portal { get; private set; }

        public LinxEspessuras() { }

        public LinxEspessuras(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxEspessuras dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_espessura = MicrovixConverters.ToInt32(dto.id_espessura);
            desc_espessura = MicrovixConverters.ToNullableString(dto.desc_espessura, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
