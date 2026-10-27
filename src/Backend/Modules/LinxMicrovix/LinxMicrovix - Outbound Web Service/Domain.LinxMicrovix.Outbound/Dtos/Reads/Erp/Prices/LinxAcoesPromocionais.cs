namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxAcoesPromocionais: as Ações Promocionais cadastradas no Microvix (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxAcoesPromocionais
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_acoes_promocionais { get; init; }
        public string? descricao { get; init; }
        public string? vigencia_inicio { get; init; }
        public string? vigencia_fim { get; init; }
        public string? observacao { get; init; }
        public string? ativa { get; init; }
        public string? excluida { get; init; }
        public string? integrada { get; init; }
        public string? qtde_integrada { get; init; }
        public string? valor_pago_franqueadora { get; init; }
        public string? id_acoes_promocionais_origem { get; init; }
        public string? timestamp { get; init; }
    }
}
