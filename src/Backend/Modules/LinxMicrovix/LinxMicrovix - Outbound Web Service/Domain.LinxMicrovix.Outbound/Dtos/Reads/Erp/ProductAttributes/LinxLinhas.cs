namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxLinhas: todos os registros do cadastro auxiliar linha (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxLinhas
    {
        public string? id_linha { get; init; }
        public string? desc_linha { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? portal { get; init; }
        public string? coeficiente_comissao { get; init; }
    }
}
