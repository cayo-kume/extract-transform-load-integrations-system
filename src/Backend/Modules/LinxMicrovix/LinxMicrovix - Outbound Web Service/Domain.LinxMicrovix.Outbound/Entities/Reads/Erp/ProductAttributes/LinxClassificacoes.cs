using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxClassificacoes, tabela linxmicrovix.LinxClassificacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_classificacao
    /// </summary>
    public class LinxClassificacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_classificacao { get; private set; }
        public string? desc_classificacao { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public bool? ativo { get; private set; }
        public int portal { get; private set; }

        public LinxClassificacoes() { }

        public LinxClassificacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxClassificacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_classificacao = MicrovixConverters.ToInt32(dto.id_classificacao);
            desc_classificacao = MicrovixConverters.ToNullableString(dto.desc_classificacao, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
