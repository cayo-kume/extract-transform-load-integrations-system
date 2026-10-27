namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxServicosDetalhes: informações dos serviços de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxServicosDetalhes
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_servico { get; init; }
        public string? preco_custo_hn { get; init; }
        public string? preco_primera_hn { get; init; }
        public string? preco_demais_hn { get; init; }
        public string? preco_custo_he { get; init; }
        public string? preco_primera_he { get; init; }
        public string? preco_demais_he { get; init; }
        public string? preco_custo_had { get; init; }
        public string? preco_primera_had { get; init; }
        public string? preco_demais_had { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? desc_config_tributaria { get; init; }
        public string? cod_trib_servico_municipio { get; init; }
        public string? timestamp { get; init; }
    }
}
