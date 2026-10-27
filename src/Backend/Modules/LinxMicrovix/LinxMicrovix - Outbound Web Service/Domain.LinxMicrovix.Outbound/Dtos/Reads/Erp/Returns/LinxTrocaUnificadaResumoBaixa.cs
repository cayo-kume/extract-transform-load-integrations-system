namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxTrocaUnificadaResumoBaixa: os dados do resumo baixa do troca unificada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxTrocaUnificadaResumoBaixa
    {
        public string? portal_baixa { get; init; }
        public string? empresa_baixa { get; init; }
        public string? cnpj_empresa_baixa { get; init; }
        public string? id_troca_baixa { get; init; }
        public string? id_troca_unificada_resumo_vendas_itens { get; init; }
        public string? data_troca_baixa { get; init; }
        public string? transacao_baixa { get; init; }
        public string? descricao_empresa_baixa { get; init; }
        public string? timestamp { get; init; }
    }
}
