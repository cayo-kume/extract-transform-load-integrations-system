namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxTrocaUnificadaResumoVendas: as vendas do troca unificada. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxTrocaUnificadaResumoVendas
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_troca_unificada_resumo_vendas { get; init; }
        public string? token { get; init; }
        public string? identificador { get; init; }
        public string? documento { get; init; }
        public string? serie { get; init; }
        public string? data_venda { get; init; }
        public string? documento_cliente { get; init; }
        public string? venda_cancelada { get; init; }
        public string? timestamp { get; init; }
    }
}
