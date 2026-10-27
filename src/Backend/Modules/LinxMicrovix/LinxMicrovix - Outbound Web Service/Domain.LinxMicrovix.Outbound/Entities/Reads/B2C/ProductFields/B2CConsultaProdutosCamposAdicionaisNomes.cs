using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaProdutosCamposAdicionaisNomes, tabela linxmicrovix.B2CConsultaProdutosCamposAdicionaisNomes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_campo (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisNomes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_campo { get; private set; }
        public int? ordem { get; private set; }
        public string? legenda { get; private set; }
        public string? tipo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosCamposAdicionaisNomes() { }

        public B2CConsultaProdutosCamposAdicionaisNomes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_campo = MicrovixConverters.ToInt32(dto.id_campo);
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            legenda = MicrovixConverters.ToNullableString(dto.legenda, 30);
            tipo = MicrovixConverters.ToNullableString(dto.tipo, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
