namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxConfiguracoesTributariasDetalhesSimplificado: o detalhamento das configurações tributarias de forma simplificada. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxConfiguracoesTributariasDetalhesSimplificado
    {
        public string? portal { get; init; }
        public string? id_config_tributaria_detalhe { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? cod_natureza_operacao { get; init; }
        public string? id_classe_fiscal { get; init; }
        public string? id_cst_icms_fiscal { get; init; }
        public string? id_csosn_fiscal { get; init; }
        public string? id_cfop_fiscal { get; init; }
        public string? ipi_credito { get; init; }
        public string? icms_credito { get; init; }
        public string? aliq_icms { get; init; }
        public string? perc_reducao_icms { get; init; }
        public string? aliquota_st { get; init; }
        public string? margem_st { get; init; }
        public string? timestamp { get; init; }
    }
}
