namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaCNPJsChave (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaCNPJsChave
    {
        public string? CNPJ { get; init; }
        public string? nome_empresa { get; init; }
        public string? id_empresas_rede { get; init; }
        public string? rede { get; init; }
        public string? portal { get; init; }
        public string? nome_portal { get; init; }
        public string? empresa { get; init; }
        public string? classificacao_portal { get; init; }
        public string? b2c { get; init; }
        public string? oms { get; init; }
        public string? b2c_marketplace { get; init; }
    }
}
