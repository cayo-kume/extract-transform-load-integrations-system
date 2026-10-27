namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxECFFormasPagamento: as formas de pagamento da ECF. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxECFFormasPagamento
    {
        public string? id_empresa_ecf_formas_pgto { get; init; }
        public string? id_ecf { get; init; }
        public string? cod_forma_pgto { get; init; }
        public string? indice_forma { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
