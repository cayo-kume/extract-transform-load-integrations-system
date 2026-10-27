using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaProdutosCamposAdicionaisValores, tabela linxmicrovix.B2CConsultaProdutosCamposAdicionaisValores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_campo_detalhe + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisValores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? id_campo_valor { get; private set; }
        public long codigoproduto { get; private set; }
        public int id_campo_detalhe { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosCamposAdicionaisValores() { }

        public B2CConsultaProdutosCamposAdicionaisValores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_campo_valor = MicrovixConverters.ToNullableInt32(dto.id_campo_valor);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            id_campo_detalhe = MicrovixConverters.ToInt32(dto.id_campo_detalhe);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
