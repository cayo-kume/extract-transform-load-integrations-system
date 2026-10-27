namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCstPisFiscal: o conteúdo da tabela cst_pis_fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCstPisFiscal
    {
        public string? portal { get; init; }
        public string? id_cst_pis_fiscal { get; init; }
        public string? cst_pis_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? excluido { get; init; }
        public string? inicio_vigencia { get; init; }
        public string? termino_vigencia { get; init; }
        public string? timestamp { get; init; }
    }
}
