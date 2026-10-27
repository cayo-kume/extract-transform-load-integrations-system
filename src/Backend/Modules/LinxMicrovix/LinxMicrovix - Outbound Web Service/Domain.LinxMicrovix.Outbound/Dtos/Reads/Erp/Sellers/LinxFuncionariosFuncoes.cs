namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFuncionariosFuncoes: o cadastro da funções de funcionários. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFuncionariosFuncoes
    {
        public string? portal { get; init; }
        public string? id_funcionarios_funcao { get; init; }
        public string? descricao { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
    }
}
