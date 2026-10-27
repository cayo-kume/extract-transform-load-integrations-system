using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaEspessuras, tabela linxmicrovix.B2CConsultaEspessuras. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_espessura
    /// </summary>
    public class B2CConsultaEspessuras
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_espessura { get; private set; }
        public string? nome_espessura { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaEspessuras() { }

        public B2CConsultaEspessuras(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaEspessuras dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_espessura = MicrovixConverters.ToInt32(dto.codigo_espessura);
            nome_espessura = MicrovixConverters.ToNullableString(dto.nome_espessura, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
