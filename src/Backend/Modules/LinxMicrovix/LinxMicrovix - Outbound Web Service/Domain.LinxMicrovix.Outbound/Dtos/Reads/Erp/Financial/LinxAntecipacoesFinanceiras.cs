namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxAntecipacoesFinanceiras: informações das antecipações financeiras do portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxAntecipacoesFinanceiras
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? empresa { get; init; }
        public string? cod_cliente { get; init; }
        public string? numero_antecipacao { get; init; }
        public string? data_antecipacao { get; init; }
        public string? previsao_entrega { get; init; }
        public string? dav_pv { get; init; }
        public string? numero_origem { get; init; }
        public string? dav_remessa { get; init; }
        public string? codigoproduto { get; init; }
        public string? quantidade { get; init; }
        public string? preco_unitario_produto { get; init; }
        public string? valor_pago_antecipacao { get; init; }
        public string? entregue { get; init; }
        public string? identificador { get; init; }
        public string? timestamp { get; init; }
        public string? cancelado { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? id_antecipacoes_financeiras_detalhes { get; init; }
        public string? id_vendas_pos_produtos { get; init; }
        public string? valor_pagar { get; init; }
        public string? identificador_faturamento_antecipado { get; init; }
    }
}
