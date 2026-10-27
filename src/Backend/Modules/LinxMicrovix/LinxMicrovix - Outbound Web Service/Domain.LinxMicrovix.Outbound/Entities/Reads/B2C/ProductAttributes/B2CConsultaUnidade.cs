using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaUnidade, tabela linxmicrovix.B2CConsultaUnidade. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + unidade (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaUnidade
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string unidade { get; private set; } = String.Empty;
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaUnidade() { }

        public B2CConsultaUnidade(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaUnidade dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            unidade = MicrovixConverters.ToNullableString(dto.unidade, 50) ?? String.Empty;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
