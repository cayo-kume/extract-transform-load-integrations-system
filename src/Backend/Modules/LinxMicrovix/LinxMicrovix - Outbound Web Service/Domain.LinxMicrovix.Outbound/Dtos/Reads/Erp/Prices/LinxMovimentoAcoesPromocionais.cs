namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoAcoesPromocionais: as Ações Promocionais realizadas nas movimentações (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoAcoesPromocionais
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? transacao { get; init; }
        public string? id_acoes_promocionais { get; init; }
        public string? desconto_item { get; init; }
        public string? quantidade { get; init; }
        public string? timestamp { get; init; }
    }
}
