namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCfopFiscal: o cadastro da CFOP Fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCfopFiscal
    {
        public string? portal { get; init; }
        public string? id_cfop_fiscal { get; init; }
        public string? cfop_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
    }
}
