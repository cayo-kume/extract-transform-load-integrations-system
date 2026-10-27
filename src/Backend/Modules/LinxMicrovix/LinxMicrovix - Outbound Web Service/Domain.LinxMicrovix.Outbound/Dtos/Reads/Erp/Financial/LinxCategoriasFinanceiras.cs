namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCategoriasFinanceiras: categoria financeira (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCategoriasFinanceiras
    {
        public string? id_categorias_financeiras { get; init; }
        public string? descricao { get; init; }
        public string? ativo { get; init; }
        public string? historico { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
