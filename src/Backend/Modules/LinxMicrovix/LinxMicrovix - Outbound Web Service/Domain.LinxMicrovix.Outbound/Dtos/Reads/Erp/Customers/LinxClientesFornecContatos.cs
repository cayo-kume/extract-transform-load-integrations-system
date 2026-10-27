namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesFornecContatos: os campos referentes ao contato do cadastro dos Clientes/Fornecedores cadastrados no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesFornecContatos
    {
        public string? portal { get; init; }
        public string? cod_cliente { get; init; }
        public string? nome_contato { get; init; }
        public string? sexo_contato { get; init; }
        public string? contatos_clientes_parentesco { get; init; }
        public string? fone1_contato { get; init; }
        public string? fone2_contato { get; init; }
        public string? celular_contato { get; init; }
        public string? email_contato { get; init; }
        public string? data_nasc_contato { get; init; }
        public string? tipo_contato { get; init; }
        public string? timestamp { get; init; }
    }
}
