namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCsosnFiscal: o cadastro de csosn fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCsosnFiscal
    {
        public string? portal { get; init; }
        public string? id_csosn_fiscal { get; init; }
        public string? csosn_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? id_csosn_fiscal_substitutiva { get; init; }
        public string? timestamp { get; init; }
    }
}
