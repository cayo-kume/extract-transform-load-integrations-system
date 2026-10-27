namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesEnderecosEntrega (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaClientesEnderecosEntrega
    {
        public string? id_endereco_entrega { get; init; }
        public string? cod_cliente_erp { get; init; }
        public string? cod_cliente_b2c { get; init; }
        public string? endereco_cliente { get; init; }
        public string? numero_rua_cliente { get; init; }
        public string? complemento_end_cli { get; init; }
        public string? cep_cliente { get; init; }
        public string? bairro_cliente { get; init; }
        public string? cidade_cliente { get; init; }
        public string? uf_cliente { get; init; }
        public string? descricao { get; init; }
        public string? principal { get; init; }
        public string? id_cidade { get; init; }
        public string? timestamp { get; init; }
        public string? id_endereco_entrega_b2c { get; init; }
        public string? portal { get; init; }
    }
}
