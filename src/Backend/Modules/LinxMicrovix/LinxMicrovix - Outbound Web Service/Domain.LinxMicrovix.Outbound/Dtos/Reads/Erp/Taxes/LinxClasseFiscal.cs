namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClasseFiscal: o cadastro da classe fiscal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClasseFiscal
    {
        public string? portal { get; init; }
        public string? id_classe_fiscal { get; init; }
        public string? descricao { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
    }
}
