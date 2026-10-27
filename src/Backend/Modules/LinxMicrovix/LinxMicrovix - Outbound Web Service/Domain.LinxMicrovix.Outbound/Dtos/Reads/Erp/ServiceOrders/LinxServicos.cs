namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxServicos: o cadastro de serviços de acordo com a loja pesquisada ou dados da estrutura mercadológica (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxServicos
    {
        public string? portal { get; init; }
        public string? cod_servico { get; init; }
        public string? nome { get; init; }
        public string? id_setor { get; init; }
        public string? desc_setor { get; init; }
        public string? id_linha { get; init; }
        public string? desc_linha { get; init; }
        public string? id_marca { get; init; }
        public string? desc_marca { get; init; }
        public string? dt_update { get; init; }
        public string? operacao_servico { get; init; }
        public string? servico_km { get; init; }
        public string? desativado { get; init; }
        public string? cod_lc11603 { get; init; }
        public string? codigo_nbs { get; init; }
        public string? dt_inclusao { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_ws { get; init; }
    }
}
