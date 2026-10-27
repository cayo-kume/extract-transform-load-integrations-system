namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoRemessasItens: os dados da tabela remessas_itens no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoRemessasItens
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_remessas_itens { get; init; }
        public string? id_remessas { get; init; }
        public string? transacao { get; init; }
        public string? qtde_total { get; init; }
        public string? qtde_venda { get; init; }
        public string? qtde_devolvido { get; init; }
        public string? qtde_pendente { get; init; }
        public string? qtde_pendente_pagamento { get; init; }
        public string? timestamp { get; init; }
    }
}
