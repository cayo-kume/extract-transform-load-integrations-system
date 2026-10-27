namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDevolucaoRemanejoFabrica: as devoluções e remanejos de fábrica. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDevolucaoRemanejoFabrica
    {
        public string? id_devolucao_remanejo_fabrica { get; init; }
        public string? id_devolucao_remanejo_fabrica_tipo { get; init; }
        public string? id_motivo_devolucao_fabrica { get; init; }
        public string? id_deposito { get; init; }
        public string? id_devolucao_remanejo_fabrica_status { get; init; }
        public string? empresa { get; init; }
        public string? fornecedor { get; init; }
        public string? cfop { get; init; }
        public string? serie { get; init; }
        public string? codigo_solicitacao { get; init; }
        public string? portal { get; init; }
        public string? data_solicitacao { get; init; }
        public string? timestamp { get; init; }
        public string? portal_2 { get; init; }
    }
}
