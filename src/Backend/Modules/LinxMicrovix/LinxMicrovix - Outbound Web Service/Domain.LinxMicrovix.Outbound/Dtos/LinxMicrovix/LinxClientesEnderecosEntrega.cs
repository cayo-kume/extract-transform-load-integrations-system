namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxClientesEnderecosEntrega
    {
        public string? id_endereco_entrega { get; set; }
        public string? cod_cliente { get; set; }
        public string? endereco_cliente { get; set; }
        public string? numero_rua_cliente { get; set; }
        public string? complemento_end_cli { get; set; }
        public string? cep_cliente { get; set; }
        public string? bairro_cliente { get; set; }
        public string? cidade_cliente { get; set; }
        public string? uf_cliente { get; set; }
        public string? descricao { get; set; }
        public string? principal { get; set; }
        public string? fone_cliente { get; set; }
        public string? fone_celular { get; set; }
        public string? timestamp { get; set; }
        public string? portal { get; set; }

        public LinxClientesEnderecosEntrega()
        {
        }

        public LinxClientesEnderecosEntrega(string? id_endereco_entrega, string? cod_cliente, string? endereco_cliente, string? numero_rua_cliente, string? complemento_end_cli, string? cep_cliente, string? bairro_cliente, string? cidade_cliente, string? uf_cliente, string? descricao, string? principal, string? fone_cliente, string? fone_celular, string? timestamp, string? portal)
        {
            this.id_endereco_entrega = id_endereco_entrega;
            this.cod_cliente = cod_cliente;
            this.endereco_cliente = endereco_cliente;
            this.numero_rua_cliente = numero_rua_cliente;
            this.complemento_end_cli = complemento_end_cli;
            this.cep_cliente = cep_cliente;
            this.bairro_cliente = bairro_cliente;
            this.cidade_cliente = cidade_cliente;
            this.uf_cliente = uf_cliente;
            this.descricao = descricao;
            this.principal = principal;
            this.fone_cliente = fone_cliente;
            this.fone_celular = fone_celular;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}
