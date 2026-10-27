namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDesfazimentosAntecipacoesDocumento: os valores registrados na tabela desfazimentos_antecipacoes_documento (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDesfazimentosAntecipacoesDocumento
    {
        public string? portal { get; init; }
        public string? id_documento_desfazimento_optico { get; init; }
        public string? empresa { get; init; }
        public string? id_desfazimentos_antecipacoes { get; init; }
        public string? id_antecipacoes_financeiras { get; init; }
        public string? cfop { get; init; }
        public string? cod_cliente { get; init; }
        public string? valor_operacao_nao_desfeita { get; init; }
        public string? texto_descricao_desfazimento { get; init; }
        public string? timestamp { get; init; }
    }
}
