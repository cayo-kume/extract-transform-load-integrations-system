using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxCores, tabela linxmicrovix.LinxCores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cor
    /// </summary>
    public class LinxCores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_cor { get; private set; }
        public string? desc_cor { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public bool? ativo { get; private set; }
        public int portal { get; private set; }

        public LinxCores() { }

        public LinxCores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxCores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_cor = MicrovixConverters.ToInt32(dto.id_cor);
            desc_cor = MicrovixConverters.ToNullableString(dto.desc_cor, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
