using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaSetores, tabela linxmicrovix.B2CConsultaSetores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_setor (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaSetores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_setor { get; private set; }
        public string? nome_setor { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaSetores() { }

        public B2CConsultaSetores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaSetores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_setor = MicrovixConverters.ToInt32(dto.codigo_setor);
            nome_setor = MicrovixConverters.ToNullableString(dto.nome_setor, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
