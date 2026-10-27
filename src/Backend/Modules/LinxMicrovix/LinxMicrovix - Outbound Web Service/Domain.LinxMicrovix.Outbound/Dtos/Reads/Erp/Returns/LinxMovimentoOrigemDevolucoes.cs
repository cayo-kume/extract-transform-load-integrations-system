namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoOrigemDevolucoes: os documentos de origem relacionados as devoluções (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoOrigemDevolucoes
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? nota_origem { get; init; }
        public string? ecf_origem { get; init; }
        public string? data_origem { get; init; }
        public string? serie_origem { get; init; }
        public string? empresa { get; init; }
        public string? timestamp { get; init; }
    }
}
