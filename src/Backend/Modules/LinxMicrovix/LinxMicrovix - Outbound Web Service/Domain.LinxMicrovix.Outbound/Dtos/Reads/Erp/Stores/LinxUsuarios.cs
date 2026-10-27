namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxUsuarios: os usuários do portal (colunas da resposta real, 26/10/2026)
    /// </summary>
    public class LinxUsuarios
    {
        public string? usuario_id { get; init; }
        public string? usuario_login { get; init; }
        public string? usuario_nome { get; init; }
        public string? usuario_email { get; init; }
        public string? portal { get; init; }
        public string? usuario_grupo_id { get; init; }
        public string? grupo_usuarios { get; init; }
        public string? usuario_supervisor { get; init; }
        public string? usuario_doc { get; init; }
        public string? vendedor { get; init; }
        public string? data_criacao { get; init; }
        public string? desativado { get; init; }
        public string? empresas { get; init; }
        public string? timestamp { get; init; }
    }
}
