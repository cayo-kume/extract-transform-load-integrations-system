namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecClasses: todas as classes vinculadas a um cliente/fornecedor (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecClasses
    {
        public string? portal { get; init; }
        public string? cod_cliente { get; init; }
        public string? cod_classe { get; init; }
        public string? nome_classe { get; init; }
        public string? timestamp { get; init; }
    }
}
