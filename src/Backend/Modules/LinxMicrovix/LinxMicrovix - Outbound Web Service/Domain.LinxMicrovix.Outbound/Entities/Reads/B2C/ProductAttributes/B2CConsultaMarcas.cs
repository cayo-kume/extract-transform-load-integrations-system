using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaMarcas, tabela linxmicrovix.B2CConsultaMarcas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_marca (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaMarcas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_marca { get; private set; }
        public string? nome_marca { get; private set; }
        public long timestamp { get; private set; }
        public string? linhas { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaMarcas() { }

        public B2CConsultaMarcas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaMarcas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_marca = MicrovixConverters.ToInt32(dto.codigo_marca);   //fora da documentação
            nome_marca = MicrovixConverters.ToNullableString(dto.nome_marca, 50);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            linhas = MicrovixConverters.ToNullableString(dto.linhas, 10);   //fora da documentação
            portal = MicrovixConverters.ToInt32(dto.portal);   //fora da documentação
        }
    }
}
