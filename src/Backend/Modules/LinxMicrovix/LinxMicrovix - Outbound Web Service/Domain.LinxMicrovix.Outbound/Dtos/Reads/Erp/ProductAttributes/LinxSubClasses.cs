namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSubClasses: o cadastro das Sub Classes dos clientes e fornecedores. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSubClasses
    {
        public string? id_subclasses { get; init; }
        public string? classe { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
