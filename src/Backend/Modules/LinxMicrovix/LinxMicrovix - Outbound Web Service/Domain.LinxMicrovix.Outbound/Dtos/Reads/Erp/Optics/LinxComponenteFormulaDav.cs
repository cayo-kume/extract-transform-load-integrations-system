namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxComponenteFormulaDav: os componentes de fórmulas relacionados ao Dav. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxComponenteFormulaDav
    {
        public string? id_componente_formula_dav { get; init; }
        public string? codigo_sistema_terceiro { get; init; }
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
