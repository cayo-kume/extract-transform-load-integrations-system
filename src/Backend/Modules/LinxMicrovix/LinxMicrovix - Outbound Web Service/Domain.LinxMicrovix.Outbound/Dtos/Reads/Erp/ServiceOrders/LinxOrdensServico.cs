namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrdensServico: as ordens de serviço (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrdensServico
    {
        public string? numero_os { get; init; }
        public string? empresa { get; init; }
        public string? data_os { get; init; }
        public string? data_envio_laboratorio { get; init; }
        public string? cancelado { get; init; }
        public string? id_laboratorio { get; init; }
        public string? id_posicao_os_ramo_optico { get; init; }
        public string? compartilhar_hub_laboratorios { get; init; }
        public string? cod_cliente_os { get; init; }
        public string? cod_vendedor { get; init; }
        public string? numero_sequencial_antecipacao_financeira { get; init; }
        public string? chave_nfe_laboratorio { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? previsao_entrega_os { get; init; }
    }
}
