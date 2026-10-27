namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaEmpresas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaEmpresas
    {
        public string? empresa { get; init; }
        public string? nome_emp { get; init; }
        public string? cnpjEmp { get; init; }
        public string? end_unidade { get; init; }
        public string? complemento_end_unidade { get; init; }
        public string? nr_rua_unidade { get; init; }
        public string? bairro_unidade { get; init; }
        public string? cep_unidade { get; init; }
        public string? cidade_unidade { get; init; }
        public string? uf_unidade { get; init; }
        public string? email_unidade { get; init; }
        public string? timestamp { get; init; }
        public string? data_criacao { get; init; }
        public string? centro_distribuicao { get; init; }
        public string? portal { get; init; }
        public string? envia_produtos_b2c { get; init; }
        public string? envia_estoque_b2c { get; init; }
        public string? integracao_oms { get; init; }
    }
}
