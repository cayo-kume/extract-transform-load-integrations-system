namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCstIcmsFiscal: o cadastro de cst icms fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCstIcmsFiscal
    {
        public string? portal { get; init; }
        public string? id_cst_icms_fiscal { get; init; }
        public string? cst_icms_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? substituicao_tributaria { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
    }
}
