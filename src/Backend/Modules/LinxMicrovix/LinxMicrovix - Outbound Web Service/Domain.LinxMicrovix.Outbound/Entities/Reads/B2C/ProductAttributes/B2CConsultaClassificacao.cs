using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaClassificacao, tabela linxmicrovix.B2CConsultaClassificacao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_classificacao
    /// </summary>
    public class B2CConsultaClassificacao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_classificacao { get; private set; }
        public string? nome_classificacao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaClassificacao() { }

        public B2CConsultaClassificacao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaClassificacao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_classificacao = MicrovixConverters.ToInt32(dto.codigo_classificacao);
            nome_classificacao = MicrovixConverters.ToNullableString(dto.nome_classificacao, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
