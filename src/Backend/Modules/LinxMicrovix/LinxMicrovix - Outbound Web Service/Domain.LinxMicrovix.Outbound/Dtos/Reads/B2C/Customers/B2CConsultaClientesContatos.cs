namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaClientesContatos (colunas da V64: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class B2CConsultaClientesContatos
    {
        public string? id_clientes_contatos { get; init; }
        public string? id_contato_b2c { get; init; }
        public string? nome_contato { get; init; }
        public string? data_nasc_contato { get; init; }
        public string? sexo_contato { get; init; }
        public string? id_parentesco { get; init; }
        public string? fone_contato { get; init; }
        public string? celular_contato { get; init; }
        public string? email_contato { get; init; }
        public string? cod_cliente_erp { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
