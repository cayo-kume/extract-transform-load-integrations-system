

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxVendedores
    {
        public string? cod_vendedor { get; private set; }
        public string? nome_vendedor { get; private set; }
        public string? tipo_vendedor { get; private set; }
        public string? end_vend_rua { get; private set; }
        public string? end_vend_numero { get; private set; }
        public string? end_vend_complemento { get; private set; }
        public string? end_vend_bairro { get; private set; }
        public string? end_vend_cep { get; private set; }
        public string? end_vend_cidade { get; private set; }
        public string? end_vend_uf { get; private set; }
        public string? fone_vendedor { get; private set; }
        public string? mail_vendedor { get; private set; }
        public string? dt_upd { get; private set; }
        public string? cpf_vendedor { get; private set; }
        public string? ativo { get; private set; }
        public string? data_admissao { get; private set; }
        public string? data_saida { get; private set; }
        public string? portal { get; private set; }
        public string? timestamp { get; private set; }
        public string? matricula { get; private set; }
        public string? id_tipo_venda { get; private set; }
        public string? descricao_tipo_venda { get; private set; }
        public string? cargo { get; private set; }

        public LinxVendedores() { }

        public LinxVendedores(
            string? cod_vendedor,
            string? nome_vendedor,
            string? tipo_vendedor,
            string? end_vend_rua,
            string? end_vend_numero,
            string? end_vend_complemento,
            string? end_vend_bairro,
            string? end_vend_cep,
            string? end_vend_cidade,
            string? end_vend_uf,
            string? fone_vendedor,
            string? mail_vendedor,
            string? dt_upd,
            string? cpf_vendedor,
            string? ativo,
            string? data_admissao,
            string? data_saida,
            string? portal,
            string? timestamp,
            string? matricula,
            string? id_tipo_venda,
            string? descricao_tipo_venda,
            string? cargo
        )
        {
            this.cod_vendedor = cod_vendedor;
            this.nome_vendedor = nome_vendedor;
            this.tipo_vendedor = tipo_vendedor;
            this.end_vend_rua = end_vend_rua;
            this.end_vend_numero = end_vend_numero;
            this.end_vend_complemento = end_vend_complemento;
            this.end_vend_bairro = end_vend_bairro;
            this.end_vend_cep = end_vend_cep;
            this.end_vend_cidade = end_vend_cidade;
            this.end_vend_uf = end_vend_uf;
            this.fone_vendedor = fone_vendedor;
            this.mail_vendedor = mail_vendedor;
            this.dt_upd = dt_upd;
            this.cpf_vendedor = cpf_vendedor;
            this.ativo = ativo;
            this.data_admissao = data_admissao;
            this.data_saida = data_saida;
            this.portal = portal;
            this.timestamp = timestamp;
            this.matricula = matricula;
            this.id_tipo_venda = id_tipo_venda;
            this.descricao_tipo_venda = descricao_tipo_venda;
            this.cargo = cargo;
        }
    }
}