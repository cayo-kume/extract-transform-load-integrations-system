namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCstCofinsFiscal: o conteúdo da tabela cst_cofins_fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCstCofinsFiscal
    {
        public string? portal { get; init; }
        public string? id_cst_cofins_fiscal { get; init; }
        public string? cst_cofins_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? excluido { get; init; }
        public string? inicio_vigencia { get; init; }
        public string? termino_vigencia { get; init; }
        public string? timestamp { get; init; }
    }
}
