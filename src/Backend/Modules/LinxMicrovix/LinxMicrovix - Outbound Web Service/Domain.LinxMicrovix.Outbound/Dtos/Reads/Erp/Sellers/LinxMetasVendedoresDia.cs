namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMetasVendedoresDia: as Metas de Vendedores das lojas por dia (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMetasVendedoresDia
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_meta { get; init; }
        public string? descricao_meta { get; init; }
        public string? data_inicial_meta { get; init; }
        public string? data_final_meta { get; init; }
        public string? dia { get; init; }
        public string? valor_meta_diaria { get; init; }
        public string? cod_vendedor { get; init; }
        public string? timestamp { get; init; }
    }
}
