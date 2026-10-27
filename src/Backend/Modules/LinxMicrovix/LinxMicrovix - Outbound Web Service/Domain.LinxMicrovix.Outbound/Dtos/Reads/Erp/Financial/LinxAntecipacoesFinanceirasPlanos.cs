namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxAntecipacoesFinanceirasPlanos: os dados da tabela antecipacoes_financeiras  e antecipacoes_financeiras_planos no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxAntecipacoesFinanceirasPlanos
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? numero_antecipacao { get; init; }
        public string? forma_pgto { get; init; }
        public string? plano { get; init; }
        public string? nome_plano { get; init; }
        public string? valor { get; init; }
        public string? cancelado { get; init; }
        public string? timestamp { get; init; }
        public string? data_criacao_antecipacao_plano { get; init; }
    }
}
