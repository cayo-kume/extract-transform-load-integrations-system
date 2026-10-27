using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaTiposCobrancaFrete, tabela linxmicrovix.B2CConsultaTiposCobrancaFrete. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_tipo_cobranca_frete (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaTiposCobrancaFrete
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_tipo_cobranca_frete { get; private set; }
        public string? nome_tipo_cobranca_frete { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaTiposCobrancaFrete() { }

        public B2CConsultaTiposCobrancaFrete(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaTiposCobrancaFrete dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_tipo_cobranca_frete = MicrovixConverters.ToInt32(dto.codigo_tipo_cobranca_frete);
            nome_tipo_cobranca_frete = MicrovixConverters.ToNullableString(dto.nome_tipo_cobranca_frete, 100);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
