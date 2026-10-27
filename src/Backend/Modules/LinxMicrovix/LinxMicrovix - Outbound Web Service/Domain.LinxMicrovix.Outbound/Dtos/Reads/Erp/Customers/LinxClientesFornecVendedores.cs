namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecVendedores: o código do vendedor associado ao cadastro do cliente, conforme a tabela clientes_fornec_vendedores. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecVendedores
    {
        public string? portal { get; init; }
        public string? cod_cliente { get; init; }
        public string? cod_vendedor { get; init; }
        public string? timestamp { get; init; }
    }
}
