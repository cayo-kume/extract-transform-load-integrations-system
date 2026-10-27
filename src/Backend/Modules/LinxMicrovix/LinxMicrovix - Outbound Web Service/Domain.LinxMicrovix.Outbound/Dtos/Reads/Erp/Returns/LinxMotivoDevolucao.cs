namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMotivoDevolucao: os motivos de devoluções cadastrados no ERP. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMotivoDevolucao
    {
        public string? portal { get; init; }
        public string? cod_motivo { get; init; }
        public string? descricao_motivo { get; init; }
        public string? cod_deposito { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
    }
}
