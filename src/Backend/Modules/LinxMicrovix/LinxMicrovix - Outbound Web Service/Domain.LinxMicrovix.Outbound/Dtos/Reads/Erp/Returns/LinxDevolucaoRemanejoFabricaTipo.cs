namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDevolucaoRemanejoFabricaTipo: o cadastro de tipos da rotina devoluções e remanejos de fábrica. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaTipo
    {
        public string? id_devolucao_remanejo_fabrica_tipo { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
