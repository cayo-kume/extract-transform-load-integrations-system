using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxUnidades, tabela linxmicrovix.LinxUnidades. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + unidade
    /// </summary>
    public class LinxUnidades
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string unidade { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxUnidades() { }

        public LinxUnidades(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxUnidades dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            unidade = MicrovixConverters.ToNullableString(dto.unidade, 50) ?? String.Empty;
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
