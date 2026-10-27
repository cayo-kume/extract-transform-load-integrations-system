namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxConfiguracoesTributarias: o cadastro de configurações tributárias da empresa (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxConfiguracoesTributarias
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? desc_config_tributaria { get; init; }
        public string? sigla_config_tributaria { get; init; }
        public string? timestamp { get; init; }
        public string? ativa { get; init; }
        public string? uf { get; init; }
        public string? sistema_tributacao { get; init; }
        public string? tipo_atividade { get; init; }
        public string? id_origem_mercadoria { get; init; }
        public string? utiliza_uso_consumo { get; init; }
        public string? id_classificacao_cest_produto { get; init; }
        public string? codigo_ws { get; init; }
    }
}
