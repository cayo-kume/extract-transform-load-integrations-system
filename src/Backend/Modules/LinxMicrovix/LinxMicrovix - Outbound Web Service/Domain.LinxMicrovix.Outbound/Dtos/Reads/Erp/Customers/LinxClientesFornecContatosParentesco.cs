namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecContatosParentesco: os parentescos cadastrados (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecContatosParentesco
    {
        public string? id_parentesco { get; init; }
        public string? descricao_parentesco { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
