namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPlanos: o cadastro dos Planos de Pagamentos do portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPlanos
    {
        public string? portal { get; init; }
        public string? plano { get; init; }
        public string? desc_plano { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? prazo_entre_parcelas { get; init; }
        public string? tipo_plano { get; init; }
        public string? indice_plano { get; init; }
        public string? cod_forma_pgto { get; init; }
        public string? forma_pgto { get; init; }
        public string? conta_central { get; init; }
        public string? tipo_transacao { get; init; }
        public string? taxa_financeira { get; init; }
        public string? dt_upd { get; init; }
        public string? desativado { get; init; }
        public string? usa_tef { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_ws { get; init; }
        public string? valor_minimo_parcela { get; init; }
        public string? gerado_config_receb_vendas { get; init; }
    }
}
