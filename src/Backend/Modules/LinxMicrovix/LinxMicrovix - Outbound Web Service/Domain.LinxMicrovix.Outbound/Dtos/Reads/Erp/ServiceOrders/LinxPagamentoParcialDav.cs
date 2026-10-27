namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPagamentoParcialDav: os pagamentos parciais da DAVs (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPagamentoParcialDav
    {
        public string? id_vendas_pos { get; init; }
        public string? valor { get; init; }
        public string? ajuste_valor_desc_acresc_plano { get; init; }
        public string? timestamp { get; init; }
        public string? id_pagamento_parcial_tmp { get; init; }
        public string? plano { get; init; }
        public string? forma_pgto { get; init; }
        public string? valor_2 { get; init; }
        public string? id_bandeira { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? credito_debito { get; init; }
        public string? portal { get; init; }
    }
}
