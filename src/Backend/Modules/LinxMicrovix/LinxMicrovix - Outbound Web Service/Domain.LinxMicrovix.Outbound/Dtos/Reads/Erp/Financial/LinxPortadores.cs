namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPortadores: os dados dos portadores. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPortadores
    {
        public string? cod_portador { get; init; }
        public string? nome_portador { get; init; }
        public string? taxa_acrescimo { get; init; }
        public string? aviso_vencimento { get; init; }
        public string? reaviso_vencimento { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
