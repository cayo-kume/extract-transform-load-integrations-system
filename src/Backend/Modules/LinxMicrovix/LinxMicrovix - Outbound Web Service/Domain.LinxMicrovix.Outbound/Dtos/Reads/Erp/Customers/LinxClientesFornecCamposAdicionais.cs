namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecCamposAdicionais: os campos adicionais preenchidos dos Clientes/Fornecedores cadastrados no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecCamposAdicionais
    {
        public string? portal { get; init; }
        public string? cod_cliente { get; init; }
        public string? campo { get; init; }
        public string? valor { get; init; }
        public string? timestamp { get; init; }
    }
}
