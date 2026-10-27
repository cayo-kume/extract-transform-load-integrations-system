namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxValesComprasEnviadosAPI: vale Compras importado via API. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxValesComprasEnviadosAPI
    {
        public string? numero_controle { get; init; }
        public string? valor_vale { get; init; }
        public string? doc_cliente { get; init; }
        public string? status_vale { get; init; }
        public string? codigo_portal_resgate { get; init; }
        public string? codigo_empresa_resgate { get; init; }
        public string? codigo_usuario_resgate { get; init; }
        public string? codigo_vale_empresa_resgate { get; init; }
        public string? data_criacao { get; init; }
        public string? timestamp { get; init; }
        public string? cnpj_empresa_resgate { get; init; }
        public string? data_resgate { get; init; }
        public string? portal { get; init; }
    }
}
