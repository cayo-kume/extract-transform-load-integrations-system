namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesEnderecosEntrega: endereço de entrega do cliente. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxClientesEnderecosEntrega
    {
        public string? id_endereco_entrega { get; init; }
        public string? cod_cliente { get; init; }
        public string? endereco_cliente { get; init; }
        public string? numero_rua_cliente { get; init; }
        public string? complemento_end_cli { get; init; }
        public string? cep_cliente { get; init; }
        public string? bairro_cliente { get; init; }
        public string? cidade_cliente { get; init; }
        public string? uf_cliente { get; init; }
        public string? descricao { get; init; }
        public string? principal { get; init; }
        public string? fone_cliente { get; init; }
        public string? fone_celular { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
