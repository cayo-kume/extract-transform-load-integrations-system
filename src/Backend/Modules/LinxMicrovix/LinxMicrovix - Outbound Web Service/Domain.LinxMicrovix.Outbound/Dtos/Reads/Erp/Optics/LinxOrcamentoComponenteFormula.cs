namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrcamentoComponenteFormula: os componentes de fórmulas relacionados ao orçamento. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrcamentoComponenteFormula
    {
        public string? id_orcamento_componente_formula { get; init; }
        public string? documento { get; init; }
        public string? codigo_produto { get; init; }
        public string? codigo_componente { get; init; }
        public string? descricao_componente { get; init; }
        public string? unidade { get; init; }
        public string? quantidade { get; init; }
        public string? valor_componente { get; init; }
        public string? lote_componente { get; init; }
        public string? data_validade_lote { get; init; }
        public string? codigo_ws { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
