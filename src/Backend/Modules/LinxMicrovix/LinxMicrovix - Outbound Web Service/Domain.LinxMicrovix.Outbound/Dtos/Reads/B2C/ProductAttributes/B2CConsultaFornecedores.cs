namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaFornecedores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaFornecedores
    {
        public string? cod_fornecedor { get; init; }
        public string? nome { get; init; }
        public string? nome_fantasia { get; init; }
        public string? tipo_pessoa { get; init; }
        public string? tipo_fornecedor { get; init; }
        public string? endereco { get; init; }
        public string? numero_rua { get; init; }
        public string? bairro { get; init; }
        public string? cep { get; init; }
        public string? cidade { get; init; }
        public string? uf { get; init; }
        public string? documento { get; init; }
        public string? fone { get; init; }
        public string? email { get; init; }
        public string? pais { get; init; }
        public string? obs { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
