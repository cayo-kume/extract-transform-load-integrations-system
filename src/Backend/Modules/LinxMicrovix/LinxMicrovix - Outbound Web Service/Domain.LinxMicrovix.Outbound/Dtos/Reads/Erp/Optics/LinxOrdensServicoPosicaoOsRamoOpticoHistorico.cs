namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrdensServicoPosicaoOsRamoOpticoHistorico: o histórico das posições das ordens de serviço do ramo optico (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrdensServicoPosicaoOsRamoOpticoHistorico
    {
        public string? id_ordens_servico_posicao_os_ramo_optico_historico { get; init; }
        public string? numero_os { get; init; }
        public string? usuario { get; init; }
        public string? timestamp { get; init; }
        public string? id_posicao_os_ramo_optico { get; init; }
        public string? data { get; init; }
        public string? observacao { get; init; }
        public string? portal { get; init; }
    }
}
