namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxGrupoLojas: as lojas do grupo (colunas da resposta real, 26/10/2026)
    /// </summary>
    public class LinxGrupoLojas
    {
        public string? CNPJ { get; init; }
        public string? nome_empresa { get; init; }
        public string? id_empresas_rede { get; init; }
        public string? rede { get; init; }
        public string? portal { get; init; }
        public string? nome_portal { get; init; }
        public string? empresa { get; init; }
        public string? lojas_proprias { get; init; }
        public string? classificacao_portal { get; init; }
        public string? emp_mod_linx_promo { get; init; }
    }
}
