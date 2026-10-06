namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaClientesEnderecosEntrega
    {
        public string? id_endereco_entrega { get; private set; }
        public string? cod_cliente_erp { get; private set; }
        public string? cod_cliente_b2c { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complemento_end_cli { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? descricao { get; private set; }
        public string? principal { get; private set; }
        public string? id_cidade { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }

        public B2CConsultaClientesEnderecosEntrega() { }

        public B2CConsultaClientesEnderecosEntrega(
            string? id_endereco_entrega,
            string? cod_cliente_erp,
            string? cod_cliente_b2c,
            string? endereco_cliente,
            string? numero_rua_cliente,
            string? complemento_end_cli,
            string? cep_cliente,
            string? bairro_cliente,
            string? cidade_cliente,
            string? uf_cliente,
            string? descricao,
            string? principal,
            string? id_cidade,
            string? timestamp,
            string? portal
        )
        {
            this.id_endereco_entrega = id_endereco_entrega;
            this.cod_cliente_erp = cod_cliente_erp;
            this.cod_cliente_b2c = cod_cliente_b2c;
            this.endereco_cliente = endereco_cliente;
            this.numero_rua_cliente = numero_rua_cliente;
            this.complemento_end_cli = complemento_end_cli;
            this.cep_cliente = cep_cliente;
            this.bairro_cliente = bairro_cliente;
            this.cidade_cliente = cidade_cliente;
            this.uf_cliente = uf_cliente;
            this.descricao = descricao;
            this.principal = principal;
            this.id_cidade = id_cidade;
            this.timestamp = timestamp;
            this.portal = portal;
        }

    }
}