namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxConhecimentoFretes: dados de conhecimento de fretes. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxConhecimentoFretes
    {
        public string? id_conhecimento { get; init; }
        public string? transportador { get; init; }
        public string? valor_frete { get; init; }
        public string? conhecimento { get; init; }
        public string? serie_frete { get; init; }
        public string? tipo_frete { get; init; }
        public string? vencto_frete { get; init; }
        public string? valor_total_notas { get; init; }
        public string? valor_lancado { get; init; }
        public string? data_emissao { get; init; }
        public string? data_entrada { get; init; }
        public string? cfop { get; init; }
        public string? aliq_icms { get; init; }
        public string? empresa { get; init; }
        public string? base_calculo_icms { get; init; }
        public string? id_cfop_fiscal { get; init; }
        public string? id_modelo_nf { get; init; }
        public string? id_cte_tipo { get; init; }
        public string? valor_icms { get; init; }
        public string? valor_pis { get; init; }
        public string? valor_cofins { get; init; }
        public string? chave_cte { get; init; }
        public string? regime_pis_cofins { get; init; }
        public string? base_pis { get; init; }
        public string? base_cofins { get; init; }
        public string? id_cst_icms_fiscal { get; init; }
        public string? id_nfe_ibge_municipio_origem { get; init; }
        public string? id_nfe_ibge_municipio_destino { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
