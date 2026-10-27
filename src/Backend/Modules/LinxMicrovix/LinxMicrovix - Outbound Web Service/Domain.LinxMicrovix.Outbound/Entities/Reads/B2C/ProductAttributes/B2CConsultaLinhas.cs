using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaLinhas, tabela linxmicrovix.B2CConsultaLinhas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_linha
    /// </summary>
    public class B2CConsultaLinhas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_linha { get; private set; }
        public string? nome_linha { get; private set; }
        public long timestamp { get; private set; }
        public string? setores { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaLinhas() { }

        public B2CConsultaLinhas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaLinhas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_linha = MicrovixConverters.ToInt32(dto.codigo_linha);
            nome_linha = MicrovixConverters.ToNullableString(dto.nome_linha, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            setores = MicrovixConverters.ToNullableString(dto.setores, 250);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
