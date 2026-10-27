namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoGiftCard: os registro de giftcard (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoGiftCard
    {
        public string? empresa { get; init; }
        public string? data_transacao { get; init; }
        public string? operacao { get; init; }
        public string? nsu_cliente { get; init; }
        public string? nsu_host { get; init; }
        public string? valor { get; init; }
        public string? json_envio { get; init; }
        public string? json_retorno { get; init; }
        public string? qtde_tentativa { get; init; }
        public string? aprovado_barramento { get; init; }
        public string? estornada { get; init; }
        public string? codigo_loja { get; init; }
        public string? identificador_movimento { get; init; }
        public string? numero_cartao { get; init; }
        public string? plano { get; init; }
        public string? ambiente_producao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
