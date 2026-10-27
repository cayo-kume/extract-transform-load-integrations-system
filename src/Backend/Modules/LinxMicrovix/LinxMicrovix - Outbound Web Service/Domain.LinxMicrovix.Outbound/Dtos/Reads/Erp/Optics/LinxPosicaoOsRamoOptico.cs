namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPosicaoOsRamoOptico: o cadastro da posição da OS do Ramo Optico (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPosicaoOsRamoOptico
    {
        public string? id_posicao_os_ramo_optico { get; init; }
        public string? descricao { get; init; }
        public string? codigo_status_padrao { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
