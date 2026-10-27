namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFechamentoCaixa (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFechamentoCaixa
    {
        public string? empresa { get; init; }
        public string? data { get; init; }
        public string? usuario { get; init; }
        public string? qtd_001 { get; init; }
        public string? qtd_005 { get; init; }
        public string? qtd_010 { get; init; }
        public string? qtd_025 { get; init; }
        public string? qtd_050 { get; init; }
        public string? qtd_1 { get; init; }
        public string? qtd_2 { get; init; }
        public string? qtd_5 { get; init; }
        public string? qtd_10 { get; init; }
        public string? qtd_20 { get; init; }
        public string? qtd_50 { get; init; }
        public string? qtd_100 { get; init; }
        public string? qtd_200 { get; init; }
        public string? total_geral { get; init; }
        public string? obs { get; init; }
        public string? total_c_vista { get; init; }
        public string? total_c_prazo { get; init; }
        public string? total_cartao { get; init; }
        public string? total_convenio { get; init; }
        public string? total_crediario { get; init; }
        public string? total_giftcard { get; init; }
        public string? total_qr_linx { get; init; }
        public string? total_cartao_credito { get; init; }
        public string? total_cartao_debito { get; init; }
        public string? total_pix { get; init; }
        public string? total_link_pagamento { get; init; }
        public string? total_link_pagamento_credito { get; init; }
        public string? total_link_pagamento_debito { get; init; }
        public string? total_vale_compra { get; init; }
        public string? vale_compras_dev { get; init; }
        public string? sangria { get; init; }
        public string? suprimentos { get; init; }
        public string? ativo { get; init; }
        public string? conferencia_efetuada { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
