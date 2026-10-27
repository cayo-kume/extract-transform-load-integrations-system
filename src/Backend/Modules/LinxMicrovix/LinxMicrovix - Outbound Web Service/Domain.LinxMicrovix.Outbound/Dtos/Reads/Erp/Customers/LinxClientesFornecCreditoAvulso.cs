namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecCreditoAvulso: os dados de crédito avulso dos clientes e fornecedores. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecCreditoAvulso
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? cod_cliente { get; init; }
        public string? controle { get; init; }
        public string? data { get; init; }
        public string? cd { get; init; }
        public string? valor { get; init; }
        public string? motivo { get; init; }
        public string? timestamp { get; init; }
        public string? identificador { get; init; }
        public string? cnpj_emp { get; init; }
    }
}
