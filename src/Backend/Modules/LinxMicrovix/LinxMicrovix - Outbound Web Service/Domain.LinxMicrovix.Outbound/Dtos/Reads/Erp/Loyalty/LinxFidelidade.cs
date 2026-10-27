namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Loyalty
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFidelidade: os dados da tabela fidelidade_parceiro_log cadastrados no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFidelidade
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_fidelidade_parceiro_log { get; init; }
        public string? data_transacao { get; init; }
        public string? operacao { get; init; }
        public string? aprovado_barramento { get; init; }
        public string? valor_monetario { get; init; }
        public string? numero_cartao { get; init; }
        public string? identificador_movimento { get; init; }
        public string? timestamp { get; init; }
    }
}
