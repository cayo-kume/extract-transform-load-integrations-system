using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxAcoesPromocionais, tabela linxmicrovix.LinxAcoesPromocionais. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_acoes_promocionais (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxAcoesPromocionais
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_acoes_promocionais { get; private set; }
        public string? descricao { get; private set; }
        public DateTime? vigencia_inicio { get; private set; }
        public DateTime? vigencia_fim { get; private set; }
        public string? observacao { get; private set; }
        public bool? ativa { get; private set; }
        public bool? excluida { get; private set; }
        public bool? integrada { get; private set; }
        public int? qtde_integrada { get; private set; }
        public decimal? valor_pago_franqueadora { get; private set; }
        public int? id_acoes_promocionais_origem { get; private set; }
        public long timestamp { get; private set; }

        public LinxAcoesPromocionais() { }

        public LinxAcoesPromocionais(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxAcoesPromocionais dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_acoes_promocionais = MicrovixConverters.ToInt32(dto.id_acoes_promocionais);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            vigencia_inicio = MicrovixConverters.ToNullableDateTime(dto.vigencia_inicio);
            vigencia_fim = MicrovixConverters.ToNullableDateTime(dto.vigencia_fim);
            observacao = String.IsNullOrEmpty(dto.observacao) ? null : dto.observacao;
            ativa = MicrovixConverters.ToNullableBoolean(dto.ativa);
            excluida = MicrovixConverters.ToNullableBoolean(dto.excluida);
            integrada = MicrovixConverters.ToNullableBoolean(dto.integrada);
            qtde_integrada = MicrovixConverters.ToNullableInt32(dto.qtde_integrada);
            valor_pago_franqueadora = MicrovixConverters.ToNullableDecimal(dto.valor_pago_franqueadora);
            id_acoes_promocionais_origem = MicrovixConverters.ToNullableInt32(dto.id_acoes_promocionais_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
