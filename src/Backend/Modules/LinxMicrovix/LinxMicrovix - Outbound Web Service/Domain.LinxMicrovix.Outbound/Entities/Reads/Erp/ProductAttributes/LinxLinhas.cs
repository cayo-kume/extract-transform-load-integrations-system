using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// LinxLinhas, tabela linxmicrovix.LinxLinhas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_linha
    /// </summary>
    public class LinxLinhas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_linha { get; private set; }
        public string? desc_linha { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public int portal { get; private set; }
        public decimal? coeficiente_comissao { get; private set; }

        public LinxLinhas() { }

        public LinxLinhas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ProductAttributes.LinxLinhas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_linha = MicrovixConverters.ToInt32(dto.id_linha);
            desc_linha = MicrovixConverters.ToNullableString(dto.desc_linha, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            portal = MicrovixConverters.ToInt32(dto.portal);
            coeficiente_comissao = MicrovixConverters.ToNullableDecimal(dto.coeficiente_comissao);
        }
    }
}
