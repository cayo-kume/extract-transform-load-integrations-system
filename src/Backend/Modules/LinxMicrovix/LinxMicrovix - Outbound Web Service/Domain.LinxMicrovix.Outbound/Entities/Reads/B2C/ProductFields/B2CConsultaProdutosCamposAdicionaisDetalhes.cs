using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaProdutosCamposAdicionaisDetalhes, tabela linxmicrovix.B2CConsultaProdutosCamposAdicionaisDetalhes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_campo_detalhe
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisDetalhes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_campo_detalhe { get; private set; }
        public int? ordem { get; private set; }
        public string? descricao { get; private set; }
        public int? id_campo { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosCamposAdicionaisDetalhes() { }

        public B2CConsultaProdutosCamposAdicionaisDetalhes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_campo_detalhe = MicrovixConverters.ToInt32(dto.id_campo_detalhe);
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 30);
            id_campo = MicrovixConverters.ToNullableInt32(dto.id_campo);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
