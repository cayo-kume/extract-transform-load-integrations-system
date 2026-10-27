namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxListagemBalanco: a listagem de balanços. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxListagemBalanco
    {
        public string? id_balanco { get; init; }
        public string? data { get; init; }
        public string? nome_arquivo { get; init; }
        public string? processado { get; init; }
        public string? data_ultimo_processamento { get; init; }
        public string? qtde_produtos { get; init; }
        public string? qtde_itens { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
