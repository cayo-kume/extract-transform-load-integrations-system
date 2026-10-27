namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDevolucaoRemanejoFabricaItem: os itens das devoluções e remanejos de fábrica. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaItem
    {
        public string? id_devolucao_remanejo_fabrica_item { get; init; }
        public string? id_devolucao_remanejo_fabrica { get; init; }
        public string? codigoproduto { get; init; }
        public string? codigo_produto_franqueadora { get; init; }
        public string? quantidade { get; init; }
        public string? codebar { get; init; }
        public string? serial { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
