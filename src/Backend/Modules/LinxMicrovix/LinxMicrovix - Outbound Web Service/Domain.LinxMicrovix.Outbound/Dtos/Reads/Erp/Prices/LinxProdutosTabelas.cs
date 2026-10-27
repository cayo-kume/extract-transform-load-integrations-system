namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosTabelas: o cadastro de Tabelas de Preços (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosTabelas
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_tabela { get; init; }
        public string? nome_tabela { get; init; }
        public string? ativa { get; init; }
        public string? timestamp { get; init; }
        public string? tipo_tabela { get; init; }
        public string? codigo_integracao_ws { get; init; }
    }
}
