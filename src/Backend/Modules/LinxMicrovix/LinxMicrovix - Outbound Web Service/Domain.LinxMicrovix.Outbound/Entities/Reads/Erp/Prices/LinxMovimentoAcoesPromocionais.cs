using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxMovimentoAcoesPromocionais, tabela linxmicrovix.LinxMovimentoAcoesPromocionais. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + transacao + id_acoes_promocionais + timestamp
    /// </summary>
    public class LinxMovimentoAcoesPromocionais
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid? identificador { get; private set; }
        public int transacao { get; private set; }
        public int id_acoes_promocionais { get; private set; }
        public decimal? desconto_item { get; private set; }
        public int? quantidade { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoAcoesPromocionais() { }

        public LinxMovimentoAcoesPromocionais(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxMovimentoAcoesPromocionais dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            id_acoes_promocionais = MicrovixConverters.ToInt32(dto.id_acoes_promocionais);
            desconto_item = MicrovixConverters.ToNullableDecimal(dto.desconto_item);
            quantidade = MicrovixConverters.ToNullableInt32(dto.quantidade);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
