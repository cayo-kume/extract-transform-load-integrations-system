namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoPlanos: os Planos de Pagamentos realizados nas movimentações (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoPlanos
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? plano { get; init; }
        public string? desc_plano { get; init; }
        public string? total { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? indice_plano { get; init; }
        public string? cod_forma_pgto { get; init; }
        public string? forma_pgto { get; init; }
        public string? tipo_transacao { get; init; }
        public string? taxa_financeira { get; init; }
        public string? ordem_cartao { get; init; }
        public string? timestamp { get; init; }
        public string? empresa { get; init; }
        public string? gerado_config_receb_vendas { get; init; }
    }
}
