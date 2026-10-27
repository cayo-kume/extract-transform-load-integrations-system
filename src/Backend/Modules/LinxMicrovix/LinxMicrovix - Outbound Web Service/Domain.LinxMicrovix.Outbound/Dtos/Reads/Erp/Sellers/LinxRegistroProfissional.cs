namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRegistroProfissional: os registros da tabela tipo_registro_profissional. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRegistroProfissional
    {
        public string? portal { get; init; }
        public string? id_tipo_registro_profissional { get; init; }
        public string? descricao_tipo_registro_profissional { get; init; }
        public string? ativo { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
    }
}
