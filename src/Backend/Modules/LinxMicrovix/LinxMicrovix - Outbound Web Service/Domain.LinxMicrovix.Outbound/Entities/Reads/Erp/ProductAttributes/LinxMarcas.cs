using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxMarcas, tabela linxmicrovix.LinxMarcas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_marca
    /// </summary>
    public class LinxMarcas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_marca { get; private set; }
        public string? desc_marca { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public int portal { get; private set; }

        public LinxMarcas() { }

        public LinxMarcas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxMarcas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_marca = MicrovixConverters.ToInt32(dto.id_marca);
            desc_marca = MicrovixConverters.ToNullableString(dto.desc_marca, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
