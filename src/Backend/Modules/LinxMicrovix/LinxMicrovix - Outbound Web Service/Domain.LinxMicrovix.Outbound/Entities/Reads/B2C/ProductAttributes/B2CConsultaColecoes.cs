using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaColecoes, tabela linxmicrovix.B2CConsultaColecoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_colecao
    /// </summary>
    public class B2CConsultaColecoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_colecao { get; private set; }
        public string? nome_colecao { get; private set; }
        public long timestamp { get; private set; }
        public string? marcas { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaColecoes() { }

        public B2CConsultaColecoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaColecoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_colecao = MicrovixConverters.ToInt32(dto.codigo_colecao);   //fora da documentação
            nome_colecao = MicrovixConverters.ToNullableString(dto.nome_colecao, 50);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            marcas = MicrovixConverters.ToNullableString(dto.marcas, 50);   //fora da documentação
            portal = MicrovixConverters.ToInt32(dto.portal);   //fora da documentação
        }
    }
}
