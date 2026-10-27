using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxTamanhos, tabela linxmicrovix.LinxTamanhos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tamanho
    /// </summary>
    public class LinxTamanhos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string id_tamanho { get; private set; } = String.Empty;
        public string? desc_tamanho { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public bool? ativo { get; private set; }
        public int portal { get; private set; }

        public LinxTamanhos() { }

        public LinxTamanhos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxTamanhos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_tamanho = MicrovixConverters.ToNullableString(dto.id_tamanho, 5) ?? String.Empty;
            desc_tamanho = MicrovixConverters.ToNullableString(dto.desc_tamanho, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
